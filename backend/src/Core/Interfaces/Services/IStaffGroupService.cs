using Domain.DTO;

namespace Domain.Interfaces.Services
{
    //Kontrakt for arbejdet med personalegrupper. Implementeringen håndhæver reglen om sletning.
    public interface IStaffGroupService
    {
        Task<IEnumerable<StaffGroupDto>> GetAllAsync();
        Task<StaffGroupDto?> GetByIdAsync(Guid id);
        Task<StaffGroupDto> CreateAsync(SaveStaffGroupDto dto);
        Task UpdateAsync(Guid id, SaveStaffGroupDto dto);
        Task DeleteAsync(Guid id);
    }
}