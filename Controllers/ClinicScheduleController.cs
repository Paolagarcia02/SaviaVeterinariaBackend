using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicScheduleController : ControllerBase
    {
        private readonly IClinicScheduleService _service;

        public ClinicScheduleController(IClinicScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<ActionResult<List<ClinicSchedule>>> GetSchedules(
            [FromHeader(Name = "FranchiseId")] int? franchiseId = null,
            [FromHeader(Name = "RoomId")] int? roomId = null,
            [FromHeader(Name = "DayOfWeek")] int? dayOfWeek = null)
        {
            return Ok(await _service.GetClinicSchedulesAsync(franchiseId, roomId, dayOfWeek));
        }

        [HttpGet("availability")]
        [Authorize(Roles = "Admin,Vet,User")]
        public async Task<ActionResult<object>> GetAvailability(
            [FromQuery][Required] int franchiseId,
            [FromQuery] int? roomId,
            [FromQuery][Required] DateTime dateTime,
            [FromQuery] int durationMinutes = 30)
        {
            var end = dateTime.AddMinutes(durationMinutes);
            var available = await _service.IsWithinOpeningHoursAsync(franchiseId, roomId, dateTime, end);
            return Ok(new { available, franchiseId, roomId, dateTime, durationMinutes });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddSchedule([FromBody] AddClinicScheduleDTO dto)
        {
            return Ok(await _service.AddClinicScheduleAsync(dto));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateSchedule([FromBody] UpdateClinicScheduleDTO dto)
        {
            return Ok(await _service.UpdateClinicScheduleAsync(dto));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteSchedule([FromQuery(Name = "IdSchedule")][Required] int id)
        {
            return Ok(await _service.DeleteClinicScheduleAsync(id));
        }
    }
}
