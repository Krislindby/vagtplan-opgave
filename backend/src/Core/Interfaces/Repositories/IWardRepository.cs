using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IWardRepository
    {
        Task<IEnumerable<Ward>> GetAllAsync();
        Task<Ward?> GetByIdAsync(Guid id);
        Task AddAsync(Ward ward);
        Task UpdateAsync(Ward ward);
        Task DeleteAsync(Ward ward);

        //Bruges til reglen om, at et afsnit med ansættelser ikke må slettes.
        Task<bool> HasEmploymentsAsync(Guid wardId);
    }
}