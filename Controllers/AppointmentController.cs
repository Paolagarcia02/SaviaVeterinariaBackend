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
        private readonly IUserService _userService;

        public AppointmentController(IAppointmentService service, IPetService petService, IUserService userService)
        {
            _service = service;
            _petService = petService;
            _userService = userService;
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

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<Appointment>>> GetUserAppointments()
        {
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(myIdString, out int myId))
            {
                return Unauthorized();
            }

            List<Appointment> list = await _service.GetAppointmentsAsync(null, null);
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
                dto.Status = "Pendiente";
                dto.Date_time = null;
                dto.Vet_id = null;
                dto.Room_id = null;

                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (!int.TryParse(myIdString, out int myId))
                {
                    return Unauthorized();
                }

                var pet = await _petService.GetOnePetAsync(dto.Pet_id);

                if (pet == null)
                {
                    return NotFound("La mascota indicada no existe.");
                }

                if (pet.Owner_id != myId)
                {
                    return StatusCode(403, "No puedes pedir citas para mascotas que no son tuyas.");
                }

                var franchiseClaim = User.Claims.FirstOrDefault(c => c.Type == "franchise_id")?.Value;
                if (int.TryParse(franchiseClaim, out int claimFranchiseId) && claimFranchiseId > 0)
                {
                    dto.Franchise_id = claimFranchiseId;
                }
                else
                {
                    var myUser = await _userService.GetOneUserAsync(myId);
                    if (myUser?.Franchise_id == null || myUser.Franchise_id <= 0)
                    {
                        return BadRequest("Tu usuario no tiene franquicia asignada para crear solicitudes.");
                    }
                    dto.Franchise_id = myUser.Franchise_id;
                }
            }

            var errors = await _service.ValidateAddAsync(dto);
            if (errors.Any())
            {
                var details = new ValidationProblemDetails(errors)
                {
                    Title = "Error de validación en la cita",
                    Status = StatusCodes.Status400BadRequest
                };
                return BadRequest(details);
            }

            // Si llegamos aquí, es porque soy el dueño O soy Admin/Vet
            bool result = await _service.AddAppointmentAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<bool>> UpdateAppointment([FromBody] UpdateAppointmentDTO dto)
        {
            var errors = await _service.ValidateUpdateAsync(dto);
            if (errors.Any())
            {
                var details = new ValidationProblemDetails(errors)
                {
                    Title = "Error de validación en la cita",
                    Status = StatusCodes.Status400BadRequest
                };
                return BadRequest(details);
            }

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
