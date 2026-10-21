using Domain.DTO;

namespace Domain.Interfaces.Services
{
    //Kontrakt for arbejdet med ansættelser. Implementeringen håndhæver reglerne om datoer, roller og overlap.
    public interface IEmploymentService
    {
        Task<IEnumerable<EmploymentDto>> GetAllAsync();
        Task<EmploymentDto?> GetByIdAsync(Guid id);
        Task<EmploymentDto> CreateAsync(SaveEmploymentDto dto);
        Task UpdateAsync(Guid id, SaveEmploymentDto dto);
        Task DeleteAsync(Guid id);
    }
}