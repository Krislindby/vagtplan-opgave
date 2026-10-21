using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Forretningslogik for ansættelser. Oversætter mellem DTO og entitet og håndhæver opgavens regler.
    public class EmploymentService : IEmploymentService
    {
        private readonly IEmploymentRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IStaffGroupRepository _staffGroupRepository;

        public EmploymentService(
            IEmploymentRepository repository,
            IUserRepository userRepository,
            IWardRepository wardRepository,
            IStaffGroupRepository staffGroupRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _wardRepository = wardRepository;
            _staffGroupRepository = staffGroupRepository;
        }

        public async Task<IEnumerable<EmploymentDto>> GetAllAsync()
        {
            var employments = await _repository.GetAllAsync();
            return employments.Select(MapToDto);
        }

        public async Task<EmploymentDto?> GetByIdAsync(Guid id)
        {
            var employment = await _repository.GetByIdAsync(id);
            return employment is null ? null : MapToDto(employment);
        }

        public async Task<EmploymentDto> CreateAsync(SaveEmploymentDto dto)
        {
            await ValidateAsync(dto, null);

            var employment = new Employment
            {
                UserId = dto.UserId,
                WardId = dto.WardId,
                StaffGroupId = dto.StaffGroupId,
                HoursPerWeek = dto.HoursPerWeek,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _repository.AddAsync(employment);

            //Hentes igen, så navnene på bruger, afsnit og personalegruppe kommer med i svaret.
            var created = await _repository.GetByIdAsync(employment.Id);
            return MapToDto(created!);
        }

        public async Task UpdateAsync(Guid id, SaveEmploymentDto dto)
        {
            var employment = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employment with id {id} not found.");

            //Ansættelsens eget id sendes med, så den ikke tæller som overlap med sig selv.
            await ValidateAsync(dto, id);

            employment.UserId = dto.UserId;
            employment.WardId = dto.WardId;
            employment.StaffGroupId = dto.StaffGroupId;
            employment.HoursPerWeek = dto.HoursPerWeek;
            employment.StartDate = dto.StartDate;
            employment.EndDate = dto.EndDate;

            await _repository.UpdateAsync(employment);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employment = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employment with id {id} not found.");

            await _repository.DeleteAsync(employment);
        }

        //Opgavens regler, samlet ét sted så opret og redigér bruger de samme.
        private async Task ValidateAsync(SaveEmploymentDto dto, Guid? excludeId)
        {
            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("End date must be after start date.");

            var user = await _userRepository.GetByIdAsync(dto.UserId)
                ?? throw new ArgumentException("The selected user does not exist.");

            if (!user.IsStaff)
                throw new ArgumentException("The selected user must have the staff role.");

            if (await _wardRepository.GetByIdAsync(dto.WardId) is null)
                throw new ArgumentException("The selected ward does not exist.");

            if (await _staffGroupRepository.GetByIdAsync(dto.StaffGroupId) is null)
                throw new ArgumentException("The selected staff group does not exist.");

            if (await _repository.HasOverlapAsync(dto.UserId, dto.StartDate, dto.EndDate, excludeId))
                throw new ArgumentException("The user already has an employment in this period.");
        }

        //Navnene tages fra de relaterede entiteter, så tabellen kan vise dem direkte.
        private static EmploymentDto MapToDto(Employment employment) => new()
        {
            Id = employment.Id,
            UserId = employment.UserId,
            UserName = employment.User!.Name,
            WardId = employment.WardId,
            WardName = employment.Ward!.Name,
            StaffGroupId = employment.StaffGroupId,
            StaffGroupName = employment.StaffGroup!.Name,
            HoursPerWeek = employment.HoursPerWeek,
            StartDate = employment.StartDate,
            EndDate = employment.EndDate
        };
    }
}