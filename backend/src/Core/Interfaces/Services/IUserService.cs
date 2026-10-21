using Domain.DTO;

namespace Domain.Interfaces.Services
{
    //Kontrakt for arbejdet med adgange. Implementeringen håndhæver reglerne om unik e-mail, roller og sletning.
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(SaveUserDto dto);
        Task UpdateAsync(Guid id, SaveUserDto dto);
        Task DeleteAsync(Guid id);
    }
}