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
            var name = dto.Name.Trim();

            if (await _repository.NameExistsAsync(name))
                throw new ArgumentException("Der findes allerede en personalegruppe med det navn.");

            var staffGroup = new StaffGroup { Name = name };
            
            await _repository.AddAsync(staffGroup);
            return MapToDto(staffGroup);
        }

        public async Task UpdateAsync(Guid id, SaveStaffGroupDto dto)
        {
            var name = dto.Name.Trim();

            var staffGroup = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Personalegruppen med id {id} blev ikke fundet.");

            if (await _repository.NameExistsAsync(name, id))
                throw new ArgumentException("Der findes allerede en personalegruppe med det navn.");


            staffGroup.Name = name;

            await _repository.UpdateAsync(staffGroup);
        }

        public async Task DeleteAsync(Guid id)
        {
            var staffGroup = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Personalegruppen med id {id} blev ikke fundet.");

            //Opgavens regel: personalegrupper med tilknyttede ansættelser må ikke slettes.
            if (await _repository.HasEmploymentsAsync(id))
                throw new ArgumentException("Personalegruppen kan ikke slettes, fordi der er tilknyttede ansættelser.");

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