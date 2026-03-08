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
    public class AdoptionApplicationController : ControllerBase
    {
        private readonly IAdoptionApplicationService _service;

        public AdoptionApplicationController(IAdoptionApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Vet,User")]
        public async Task<ActionResult<List<AdoptionApplication>>> GetAdoptionApplications(
            [FromHeader(Name = "UserId")] int? userId = null,
            [FromHeader(Name = "Status")] string status = "")
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            // Si es User, ignoramos el filtro que mande y FORZAMOS su ID
            if (myRole == "User" && int.TryParse(myIdString, out int myId))
            {
                userId = myId;
            }

            List<AdoptionApplication> list = await _service.GetAdoptionApplicationsAsync(userId, status);
            return Ok(list);
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<AdoptionApplication>>> GetUserAdoptionApplications()
        {
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(myIdString, out int myId))
            {
                return Unauthorized();
            }

            List<AdoptionApplication> list = await _service.GetAdoptionApplicationsAsync(myId, "");
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<AdoptionApplication>> GetOneAdoptionApplication(int id)
        {
            var application = await _service.GetOneAdoptionApplicationAsync(id);
            if (application == null) return NotFound("Solicitud no encontrada.");

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Si es User y la solicitud no es suya -> PROHIBIDO
            if (myRole == "User" && int.TryParse(myIdString, out int myId))
            {
                if (application.User_id != myId)
                {
                    return StatusCode(403, "No puedes ver las solicitudes de otros usuarios.");
                }
            }

            return Ok(application);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<bool>> AddAdoptionApplication([FromBody] AddAdoptionApplicationDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Si es User, forzamos su User_id y el Status a "Pendiente"
            if (myRole == "User" && int.TryParse(myIdString, out int myId))
            {
                dto.User_id = myId;
                dto.Status = "Pendiente"; // Un usuario siempre crea en pendiente
            }

            bool result = await _service.AddAdoptionApplicationAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateAdoptionApplication([FromBody] UpdateAdoptionApplicationDTO dto)
        {
            bool result = await _service.UpdateAdoptionApplicationAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<bool>> DeleteAdoptionApplication([FromQuery(Name = "IdApplication")][Required] int id)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si es User, hay que verificar que sea suya antes de borrar
            if (myRole == "User")
            {
                var application = await _service.GetOneAdoptionApplicationAsync(id);
                if (application == null) return NotFound();

                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    if (application.User_id != myId) return StatusCode(403, "No puedes borrar esto.");
                }
            }

            bool result = await _service.DeleteAdoptionApplicationAsync(id);
            return Ok(result);
        }
    }
}