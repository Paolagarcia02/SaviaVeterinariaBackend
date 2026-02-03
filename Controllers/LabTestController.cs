using System.ComponentModel.DataAnnotations;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SaviaVetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _labTestService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;

        public LabTestController(ILabTestService labTestService, IAppointmentService appointmentService, IPetService petService)
        {
            _labTestService = labTestService;
            _appointmentService = appointmentService;
            _petService = petService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<List<LabTest>>> GetLabTests(
            [FromHeader(Name = "AppointmentId")] int? appointmentId = null,
            [FromHeader(Name = "Status")] string status = "")
        {
            var list = await _labTestService.GetLabTestsAsync(appointmentId, status);

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // SI ES USER: Filtramos manualmente haciendo el "doble salto"
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    var myTests = new List<LabTest>();

                    foreach (var test in list)
                    {
                        var appointment = await _appointmentService.GetOneAppointmentAsync(test.Appointment_id);
                        
                        if (appointment != null)
                        {
                            var pet = await _petService.GetOnePetAsync(appointment.Pet_id);

                            if (pet != null && pet.Owner_id == myId)
                            {
                                myTests.Add(test);
                            }
                        }
                    }
                    return Ok(myTests);
                }
            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<LabTest>> GetOneLabTest(int id)
        {
            var item = await _labTestService.GetOneLabTestAsync(id);
            if (item == null) return NotFound();

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    var appointment = await _appointmentService.GetOneAppointmentAsync(item.Appointment_id);
                    if (appointment == null) return StatusCode(403); 

                    var pet = await _petService.GetOnePetAsync(appointment.Pet_id);

                    if (pet == null || pet.Owner_id != myId)
                    {
                        return StatusCode(403, "No puedes ver resultados de mascotas ajenas.");
                    }
                }
            }
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Vet")] 
        public async Task<ActionResult<bool>> AddLabTest([FromBody] AddLabTestDTO dto)
        {
            bool result = await _labTestService.AddLabTestAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<bool>> UpdateLabTest([FromBody] UpdateLabTestDTO dto)
        {
            bool result = await _labTestService.UpdateLabTestAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteLabTest([FromQuery(Name = "IdTest")][Required] int id)
        {
            bool result = await _labTestService.DeleteLabTestAsync(id);
            return Ok(result);
        }
    }
}