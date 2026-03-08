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
    public class PetController : ControllerBase
    {
        private readonly IPetService _service;
        private readonly IImageService _imageService;

        public PetController(IPetService service, IImageService imageService)
        {
            _service = service;
            _imageService = imageService;
        }

        [HttpGet]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<List<Pet>>> GetPets(
            [FromHeader(Name = "OwnerId")] int? ownerId = null,
            [FromHeader(Name = "Species")] string species = "")
        {
            List<Pet> list = await _service.GetPetsAsync(ownerId, species);

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // SI ES USER: puede ver sus mascotas y también las que están en adopción
            if (myRole == "User" && int.TryParse(myIdString, out int myId))
            {
                list = list.Where(p => p.Owner_id == myId || p.Status == "En Adopción").ToList();
            }

            return Ok(list);
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<Pet>>> GetUserPets()
        {
            var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(myIdString, out int myId))
            {
                return Unauthorized();
            }

            List<Pet> list = await _service.GetPetsAsync(myId, "");
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<Pet>> GetOne(int id)
        {
            var item = await _service.GetOnePetAsync(id);
            if (item == null) return NotFound();

            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si es User, solo puede ver su mascota o una mascota en adopción
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    if (item.Owner_id != myId && item.Status != "En Adopción")
                    {
                        return StatusCode(403, "No puedes ver la ficha de una mascota ajena.");
                    }
                }
            }
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Vet,User")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult<bool>> AddPet([FromForm] AddPetDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si es User, forzamos su Owner_id
            if (myRole == "User")
            {
                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    dto.Owner_id = myId;
                }
            }

            bool result = await _service.AddPetAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<bool>> UpdatePet([FromBody] UpdatePetDTO dto)
        {
            var myRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si es User, hay que verificar que la mascota que intenta editar es suya
            if (myRole == "User")
            {
                var originalPet = await _service.GetOnePetAsync(dto.Pet_id);
                if (originalPet == null) return NotFound();

                var myIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(myIdString, out int myId))
                {
                    if (originalPet.Owner_id != myId)
                    {
                        return StatusCode(403, "No puedes editar mascotas de otros.");
                    }
                }
            }

            bool result = await _service.UpdatePetAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Vet")]
        public async Task<ActionResult<bool>> DeletePet([FromQuery(Name = "IdPet")][Required] int id)
        {
            bool result = await _service.DeletePetAsync(id);
            return Ok(result);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            // _logger.LogInformation($"Received file {file.FileName} - size: {file.Length} bytes");

            var imageUrl = await _imageService.UploadImageAsync(file);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Error al cargar la imagen.");
            }

            return Ok(new { Url = imageUrl });
        }
    }
}
