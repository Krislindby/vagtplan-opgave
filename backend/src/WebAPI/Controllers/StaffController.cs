using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //API-endpoint for den skrivebeskyttede personaleoversigt.
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;

        public StaffController(IStaffService service)
        {
            _service = service;
        }

        //Perioden sendes som query-parameter: /api/staff?planningPeriodId=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetByPeriod(
            [FromQuery] Guid planningPeriodId)
        {
            try
            {
                return Ok(await _service.GetByPeriodAsync(planningPeriodId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}