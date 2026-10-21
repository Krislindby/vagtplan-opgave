using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);

        //Bruges til reglen om, at e-mail skal være unik.
        //Ved redigering: send brugerens eget id i excludeId, så den ikke tæller som dublet af sig selv.
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);

        //Bruges til reglen om, at en bruger med ansættelser ikke må slettes.
        Task<bool> HasEmploymentsAsync(Guid userId);
    }
}