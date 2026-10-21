using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlanningPeriodRepository : IPlanningPeriodRepository
    {
        private readonly VagtplanDbContext _context;
        public PlanningPeriodRepository(VagtplanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlanningPeriod>> GetAllAsync()
            => await _context.PlanningPeriods
            .OrderBy(p => p.StartDate)
            .ToListAsync();

        public async Task<PlanningPeriod?> GetByIdAsync(Guid id)
            => await _context.PlanningPeriods.FindAsync(id);

        public async Task AddAsync(PlanningPeriod period)
        {
            _context.PlanningPeriods.Add(period);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlanningPeriod period)
        {
            _context.PlanningPeriods.Update(period);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(PlanningPeriod period)
        {
            _context.PlanningPeriods.Remove(period);
            await _context.SaveChangesAsync();
        }

        //Perioder overlapper, hvis den ene begynder før den anden slutter - begge veje.
        //excludeId udelader perioden selv, så den ikke støder mod sig selv ved redigering.
        public async Task<bool> HasOverlapAsync(DateOnly startDate, DateOnly endDate, Guid? excludeId = null)
            => await _context.PlanningPeriods
                .Where(p => excludeId == null || p.Id != excludeId)
                .AnyAsync(p => p.StartDate <= endDate && startDate <= p.EndDate);
    }
}
