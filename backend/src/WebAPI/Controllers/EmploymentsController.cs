using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //API-endpoints for ansættelser. Oversætter servicens resultater og fejl til HTTP-svar.
    [ApiController]
    [Route("api/[controller]")]
    public class EmploymentsController : ControllerBase
    {
        private readonly IEmploymentService _service;

        public EmploymentsController(IEmploymentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmploymentDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<EmploymentDto>> GetById(Guid id)
        {
            var employment = await _service.GetByIdAsync(id);
            return employment is null ? NotFound() : Ok(employment);
        }

        [HttpPost]
        public async Task<ActionResult<EmploymentDto>> Create(SaveEmploymentDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaveEmploymentDto dto)
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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
        }
    }
}