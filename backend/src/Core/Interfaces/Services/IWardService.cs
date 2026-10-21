using Domain.DTO;

namespace Domain.Interfaces.Services
{
    //Kontrakt for arbejdet med afsnit. Implementeringen håndhæver reglen om sletning.
    public interface IWardService
    {
        Task<IEnumerable<WardDto>> GetAllAsync();
        Task<WardDto?> GetByIdAsync(Guid id);
        Task<WardDto> CreateAsync(SaveWardDto dto);
        Task UpdateAsync(Guid id, SaveWardDto dto);
        Task DeleteAsync(Guid id);
    }
}