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

        //Bruges til reglen om, at to afsnit ikke må have samme farve.
        Task<bool> ColorExistsAsync(string color, Guid? excludeId = null);

        //Bruges til reglen om, at to afsnit ikke må have samme navn.
        Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    }
}