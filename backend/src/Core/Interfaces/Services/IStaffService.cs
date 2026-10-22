using Domain.DTO;

namespace Domain.Interfaces.Services
{
    //Kontrakt for den skrivebeskyttede personaleoversigt.
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetByPeriodAsync(Guid planningPeriodId);
    }
}