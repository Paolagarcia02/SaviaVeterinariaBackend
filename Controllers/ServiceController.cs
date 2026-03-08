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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Service>>> GetServices(
            [FromHeader(Name = "Name")] string name = "")
        {
            List<Service> list = await _service.GetServicesAsync(name);
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Service>> GetOneService(int id)
        {
            var item = await _service.GetOneServiceAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddService([FromBody] AddServiceDTO dto)
        {
            bool result = await _service.AddServiceAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateService([FromBody] UpdateServiceDTO dto)
        {
            bool result = await _service.UpdateServiceAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteService([FromQuery(Name = "IdService")][Required] int id)
        {
            bool result = await _service.DeleteServiceAsync(id);
            return Ok(result);
        }
    }
}