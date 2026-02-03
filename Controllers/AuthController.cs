using Microsoft.AspNetCore.Mvc;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Services;

namespace SaviaVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos el servicio que acabamos de crear
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDtoIn loginDto)
        {
            var token = await _authService.Login(loginDto);

            if (token == null)
            {
                // Si devuelve null es que el email o pass están mal
                return Unauthorized("Credenciales incorrectas");
            }

            // Si hay token, lo devolvemos en un JSON
            return Ok(new { Token = token });
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDtoIn userDto)
        {
            try
            {
                var token = await _authService.Register(userDto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Por si el email ya existe o falla la BDD
                return BadRequest("Error al registrar: " + ex.Message);
            }
        }
    }
}