using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IEmploymentRepository
    {
        Task<IEnumerable<Employment>> GetAllAsync();
        Task<Employment?> GetByIdAsync(Guid id);
        Task AddAsync(Employment employment);
        Task UpdateAsync(Employment employment);
        Task DeleteAsync(Employment employment);

        //Bruges til reglen om, at en brugers ansættelser ikke må overlappe hinanden.
        //Ved redigering: send ansættelsens eget id i excludeId, så den ikke tæller med.
        Task<bool> HasOverlapAsync(Guid userId, DateOnly startDate, DateOnly endDate, Guid? excludeId = null);
        
        //Henter de ansættelser, der overlapper et tidsrum. Bruges af Personale-siden.
        Task<IEnumerable<Employment>> GetOverlappingAsync(DateOnly startDate, DateOnly endDate);
    }
}