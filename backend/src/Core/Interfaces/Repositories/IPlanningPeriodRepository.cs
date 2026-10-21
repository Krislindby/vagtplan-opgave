using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IPlanningPeriodRepository
    {
        Task<IEnumerable<PlanningPeriod>> GetAllAsync();
        Task<PlanningPeriod?> GetByIdAsync(Guid id);
        Task AddAsync (PlanningPeriod period);
        Task UpdateAsync (PlanningPeriod period);
        Task DeleteAsync(PlanningPeriod period);

        //Svarer på, om et tidsrum overlapper en eksisterende periode.
        //Ved redigering: send periodens eget id i excludeId, så den ikke tæller med og kan overlappe sig selv.
        Task<bool> HasOverlapAsync(DateOnly startDate, DateOnly endDate, Guid? excludeId = null);
    }
}
