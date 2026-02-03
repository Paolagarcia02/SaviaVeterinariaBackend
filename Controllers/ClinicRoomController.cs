using System.ComponentModel.DataAnnotations;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SaviaVetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicRoomController : ControllerBase
    {
        private readonly IClinicRoomService _service;

        public ClinicRoomController(IClinicRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<List<ClinicRoom>>> GetClinicRooms(
            [FromHeader(Name = "FranchiseId")] int? franchiseId = null,
            [FromHeader(Name = "RoomType")] string roomType = "")
        {
            List<ClinicRoom> list = await _service.GetClinicRoomsAsync(franchiseId, roomType);
            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<ClinicRoom>> GetOneClinicRoom(int id)
        {
            var item = await _service.GetOneClinicRoomAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddClinicRoom([FromBody] AddClinicRoomDTO dto)
        {
            bool result = await _service.AddClinicRoomAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateClinicRoom([FromBody] UpdateClinicRoomDTO dto)
        {
            bool result = await _service.UpdateClinicRoomAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteClinicRoom([FromQuery(Name = "IdRoom")][Required] int id)
        {
            bool result = await _service.DeleteClinicRoomAsync(id);
            return Ok(result);
        }
    }
}