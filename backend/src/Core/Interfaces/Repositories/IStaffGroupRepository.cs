using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IStaffGroupRepository
    {
        Task<IEnumerable<StaffGroup>> GetAllAsync();
        Task<StaffGroup?> GetByIdAsync(Guid id);
        Task AddAsync(StaffGroup staffGroup);
        Task UpdateAsync(StaffGroup staffGroup);
        Task DeleteAsync(StaffGroup staffGroup);

        //Bruges til reglen om, at en personalegruppe med ansættelser ikke må slettes.
        Task<bool> HasEmploymentsAsync(Guid staffGroupId);
    }
}