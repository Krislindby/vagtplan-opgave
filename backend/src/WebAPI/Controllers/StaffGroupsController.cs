using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //API-endpoints for personalegrupper. Oversætter servicens resultater og fejl til HTTP-svar.
    [ApiController]
    [Route("api/[controller]")]
    public class StaffGroupsController : ControllerBase
    {
        private readonly IStaffGroupService _service;

        public StaffGroupsController(IStaffGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffGroupDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffGroupDto>> GetById(Guid id)
        {
            var staffGroup = await _service.GetByIdAsync(id);
            return staffGroup is null ? NotFound() : Ok(staffGroup);
        }

        [HttpPost]
        public async Task<ActionResult<StaffGroupDto>> Create(SaveStaffGroupDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaveStaffGroupDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}