using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WardRepository : IWardRepository
    {
        private readonly VagtplanDbContext _context;

        public WardRepository(VagtplanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> GetAllAsync()
            => await _context.Wards.OrderBy(w => w.Name).ToListAsync();

        public async Task<Ward?> GetByIdAsync(Guid id)
            => await _context.Wards.FindAsync(id);

        public async Task AddAsync(Ward ward)
        {
            _context.Wards.Add(ward);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ward ward)
        {
            _context.Wards.Update(ward);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Ward ward)
        {
            _context.Wards.Remove(ward);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasEmploymentsAsync(Guid wardId)
            => await _context.Employments.AnyAsync(e => e.WardId == wardId);
    }
}