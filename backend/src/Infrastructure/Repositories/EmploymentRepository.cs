using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmploymentRepository : IEmploymentRepository
    {
        private readonly VagtplanDbContext _context;

        public EmploymentRepository(VagtplanDbContext context)
        {
            _context = context;
        }

        //Include henter de tre relaterede entiteter med, så tabellen kan vise navne i stedet for id'er.
        public async Task<IEnumerable<Employment>> GetAllAsync()
            => await _context.Employments
                .Include(e => e.User)
                .Include(e => e.Ward)
                .Include(e => e.StaffGroup)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

        public async Task<Employment?> GetByIdAsync(Guid id)
            => await _context.Employments
                .Include(e => e.User)
                .Include(e => e.Ward)
                .Include(e => e.StaffGroup)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task AddAsync(Employment employment)
        {
            _context.Employments.Add(employment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employment employment)
        {
            _context.Employments.Update(employment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employment employment)
        {
            _context.Employments.Remove(employment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(Guid userId, DateOnly startDate, DateOnly endDate, Guid? excludeId = null)
            => await _context.Employments
                .Where(e => e.UserId == userId)
                .Where(e => excludeId == null || e.Id != excludeId)
                .AnyAsync(e => e.StartDate <= endDate && startDate <= e.EndDate);
    }
}