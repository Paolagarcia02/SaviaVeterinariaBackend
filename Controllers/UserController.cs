using System.ComponentModel.DataAnnotations;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace SaviaVetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Vet")]
        public async Task<ActionResult<List<User>>> GetUsers(
            [FromHeader(Name = "Role")] string role = "",
            [FromHeader(Name = "Name")] string name = "")
        {
            List<User> list = await _service.GetUsersAsync(role, name);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetOne(int id)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Si eres 'User', comprobamos que sea TU perfil
            if (myRole == "User")
            {
                // TryParse es más seguro que Parse por si viene null
                if (int.TryParse(myIdString, out int myId))
                {
                    if (myId != id)
                    {
                        return StatusCode(403, "No tienes permiso para ver el perfil de otros usuarios.");
                    }
                }
            }

            var user = await _service.GetOneUserAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            return Ok(user);
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> AddUser([FromBody] AddUserDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            //  Registro público (Nadie logueado)
            if (string.IsNullOrEmpty(myRole))
            {
                dto.Role = "User"; 
            }
            // Estoy logueado pero NO soy Admin (ej: un Vet intentando crear usuarios)
            else if (myRole != "Admin")
            {
                return StatusCode(403, "Solo los administradores pueden crear cuentas manualmente.");
            }
            // Soy Admin.
            bool result = await _service.AddUserAsync(dto);
            return Ok(result);
        }
        

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<bool>> UpdateUser([FromBody] UpdateUserDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(myIdString, out int myId))
            {
                return BadRequest("Token inválido.");
            }

            // Buscamos a quién queremos editar
            var targetUser = await _service.GetOneUserAsync(dto.User_id);
            if (targetUser == null) return NotFound("Usuario no encontrado.");

            // user
            if (myRole == "User")
            {
                if (targetUser.User_id != myId)
                {
                    return StatusCode(403, "No puedes editar a otros usuarios.");
                }

                if (!string.IsNullOrEmpty(dto.Role) && dto.Role != "User")
                {
                    return StatusCode(403, "No puedes cambiar tu propio rol.");
                }
                
                dto.Role = "User"; 
            }

            // VETERINARIO
            if (myRole == "Vet")
            {
                if (targetUser.Role == "Admin" || targetUser.Role == "Vet")
                {
                    return StatusCode(403, "Los veterinarios solo pueden editar fichas de clientes.");
                }

                if (dto.Role == "Admin")
                {
                    return StatusCode(403, "No puedes asignar roles administrativos.");
                }
            }

            bool result = await _service.UpdateUserAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteUser([FromQuery(Name = "IdUser")][Required] int id)
        {
            bool result = await _service.DeleteUserAsync(id);
            return Ok(result);
        }
    }


}