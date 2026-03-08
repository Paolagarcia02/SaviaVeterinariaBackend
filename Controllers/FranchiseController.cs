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
    public class FranchiseController : ControllerBase
    {
        private readonly IFranchiseService _service;

        public FranchiseController(IFranchiseService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Franchise>>> GetFranchises(
            [FromHeader(Name = "Name")] string name = "")
        {
            List<Franchise> list = await _service.GetFranchisesAsync(name);
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Franchise>> GetOneFranchise(int id)
        {
            var item = await _service.GetOneFranchiseAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddFranchise([FromBody] AddFranchiseDTO dto)
        {
            bool result = await _service.AddFranchiseAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateFranchise([FromBody] UpdateFranchiseDTO dto)
        {
            bool result = await _service.UpdateFranchiseAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteFranchise([FromQuery(Name = "IdFranchise")][Required] int id)
        {
            bool result = await _service.DeleteFranchiseAsync(id);
            return Ok(result);
        }
    }
}