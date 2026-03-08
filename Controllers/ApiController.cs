using Microsoft.AspNetCore.Mvc;
using SaviaVetAPI.Services;

namespace SaviaVetAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ApiService _apiService;

        public ApiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Este es TU endpoint: GET https://localhost:7000/api/proxy/posts
        /*[HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _apiService.GetExternalPostsAsync();
            // Aquí podrías filtrar o modificar los posts antes de enviarlos a Vue
            return Ok(posts);
        }*/

        [HttpGet("DailyQuote")]
        public async Task<IActionResult> GetConsejo()
        {
            var frase = await _apiService.GetCatFactAsync();
            return Ok(new { mensaje = frase });
        }
    }
}
