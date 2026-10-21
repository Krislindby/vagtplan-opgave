using Domain.DTO.PlanningPeriods;

namespace Domain.Interfaces.Services
{
    //Kontrakt for arbejdet med perioder. Implementeringen håndhæver reglerne om datorækkefølge og overlap.
    public interface IPlanningPeriodService
    {
        Task<IEnumerable<PlanningPeriodDto>> GetAllAsync();
        Task<PlanningPeriodDto?> GetByIdAsync(Guid id);
        Task<PlanningPeriodDto> CreateAsync(SavePlanningPeriodDto dto);
        Task UpdateAsync(Guid id, SavePlanningPeriodDto dto);
        Task DeleteAsync(Guid id);
    }
}