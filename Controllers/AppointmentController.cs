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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;
        private readonly IPetService _petService;

        public AppointmentController(IAppointmentService service, IPetService petService)
        {
            _service = service;
            _petService = petService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<List<Appointment>>> GetAppointments(
            [FromHeader(Name = "Date")] DateTime? date = null,
            [FromHeader(Name = "VetId")] int? vetId = null)
        {
            List<Appointment> list = await _service.GetAppointmentsAsync(date, vetId);

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // SI ES USER: FILTRAMOS SU MASCOTA
            if (myRole == "User" && int.TryParse(myIdString, out int myId))
            {
                var userAppointments = new List<Appointment>();

                foreach (var appointment in list)
                {
                    var pet = await _petService.GetOnePetAsync(appointment.Pet_id);
                    if (pet != null && pet.Owner_id == myId)
                    {
                        userAppointments.Add(appointment);
                    }
                }
                return Ok(userAppointments);
            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<Appointment>> GetOneAppointment(int id)
        {
            var item = await _service.GetOneAppointmentAsync(id);
            if (item == null) return NotFound();

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            
            // SEGURIDAD: Verificar dueño
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    // Buscamos la mascota para ver si es mía
                    var pet = await _petService.GetOnePetAsync(item.Pet_id);
                    if (pet == null || pet.Owner_id != myId) 
                    {
                        return StatusCode(403, "No puedes ver citas de mascotas ajenas.");
                    }
                }
            }
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<bool>> AddAppointment([FromBody] AddAppointmentDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si es un Usuario normal, hay que ver qué está pidiendo
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (int.TryParse(myIdString, out int myId))
                {
                    var pet = await _petService.GetOnePetAsync(dto.Pet_id);

                    if (pet == null)
                    {
                        return NotFound("La mascota indicada no existe.");
                    }

                    if (pet.Owner_id != myId)
                    {
                        return StatusCode(403, "No puedes pedir citas para mascotas que no son tuyas.");
                    }
                }
            }

            // Si llegamos aquí, es porque soy el dueño O soy Admin/Vet
            bool result = await _service.AddAppointmentAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<bool>> UpdateAppointment([FromBody] UpdateAppointmentDTO dto)
        {
            bool result = await _service.UpdateAppointmentAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<bool>> DeleteAppointment([FromQuery(Name = "IdAppointment")][Required] int id)
        {
            var item = await _service.GetOneAppointmentAsync(id);
            if (item == null) return NotFound();

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // SI ES USER: Verificamos propiedad a través de la mascota
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    // Usamos el PetService para saber de quién es la mascota
                    var pet = await _petService.GetOnePetAsync(item.Pet_id);
                    
                    // Si la mascota no es mía, error
                    if (pet == null || pet.Owner_id != myId)
                    {
                        return StatusCode(403, "No puedes cancelar citas de otros.");
                    }
                }
            }

            bool result = await _service.DeleteAppointmentAsync(id);
            return Ok(result);
        }
    }
}