using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StaffGroupRepository : IStaffGroupRepository
    {
        private readonly VagtplanDbContext _context;

        public StaffGroupRepository(VagtplanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffGroup>> GetAllAsync()
            => await _context.StaffGroups.OrderBy(s => s.Name).ToListAsync();

        public async Task<StaffGroup?> GetByIdAsync(Guid id)
            => await _context.StaffGroups.FindAsync(id);

        public async Task AddAsync(StaffGroup staffGroup)
        {
            _context.StaffGroups.Add(staffGroup);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StaffGroup staffGroup)
        {
            _context.StaffGroups.Update(staffGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StaffGroup staffGroup)
        {
            _context.StaffGroups.Remove(staffGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasEmploymentsAsync(Guid staffGroupId)
            => await _context.Employments.AnyAsync(e => e.StaffGroupId == staffGroupId);

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
            => await _context.StaffGroups
                .Where(s => excludeId == null || s.Id != excludeId)
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }
}