using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Forretningslogik for personalegrupper. Oversætter mellem DTO og entitet og håndhæver opgavens regler.
    public class StaffGroupService : IStaffGroupService
    {
        private readonly IStaffGroupRepository _repository;

        public StaffGroupService(IStaffGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StaffGroupDto>> GetAllAsync()
        {
            var staffGroups = await _repository.GetAllAsync();
            return staffGroups.Select(MapToDto);
        }

        public async Task<StaffGroupDto?> GetByIdAsync(Guid id)
        {
            var staffGroup = await _repository.GetByIdAsync(id);
            return staffGroup is null ? null : MapToDto(staffGroup);
        }

        public async Task<StaffGroupDto> CreateAsync(SaveStaffGroupDto dto)
        {
            var staffGroup = new StaffGroup { Name = dto.Name };

            await _repository.AddAsync(staffGroup);
            return MapToDto(staffGroup);
        }

        public async Task UpdateAsync(Guid id, SaveStaffGroupDto dto)
        {
            var staffGroup = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff group with id {id} not found.");

            staffGroup.Name = dto.Name;

            await _repository.UpdateAsync(staffGroup);
        }

        public async Task DeleteAsync(Guid id)
        {
            var staffGroup = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Staff group with id {id} not found.");

            //Opgavens regel: personalegrupper med tilknyttede ansættelser må ikke slettes.
            if (await _repository.HasEmploymentsAsync(id))
                throw new ArgumentException("The staff group cannot be deleted because it has employments.");

            await _repository.DeleteAsync(staffGroup);
        }

        //CreatedAt kommer ikke med ud til klienten.
        private static StaffGroupDto MapToDto(StaffGroup staffGroup) => new()
        {
            Id = staffGroup.Id,
            Name = staffGroup.Name
        };
    }
}