using Domain.DTO.PlanningPeriods;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Forretningslogik for perioder. Oversætter mellem DTO og entitet og håndhæver opgavens regler. 
    public class PlanningPeriodService : IPlanningPeriodService
    {
        private readonly IPlanningPeriodRepository _repository;

        public PlanningPeriodService(IPlanningPeriodRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PlanningPeriodDto>> GetAllAsync()
        {
            var periods = await _repository.GetAllAsync();
            return periods.Select(MapToDto);
        }

        public async Task<PlanningPeriodDto?> GetByIdAsync(Guid id)
        {
            var period = await _repository.GetByIdAsync(id);
            return period is null ? null : MapToDto(period);
        }

        public async Task<PlanningPeriodDto> CreateAsync(SavePlanningPeriodDto dto)
        {
            
            await ValidateAsync(dto, null);
            var period = new PlanningPeriod
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            await _repository.AddAsync(period);
            return MapToDto(period);
        }

        public async Task UpdateAsync(Guid id, SavePlanningPeriodDto dto)
        {
            var period = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Planning period with id {id} not found.");

            //Periodens eget id sendes med, så den ikke tæller som overlap med sig selv.
            await ValidateAsync(dto, id);

            period.Name = dto.Name;
            period.StartDate = dto.StartDate;
            period.EndDate = dto.EndDate;

            await _repository.UpdateAsync(period);
        }

        public async Task DeleteAsync(Guid id)
        {
            var period = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Perioden med id {id} blev ikke fundet.");

            await _repository.DeleteAsync(period);
        }

        //Opgavens to regler, samlet ét sted så opret og redigér bruger de samme.
        private async Task ValidateAsync(SavePlanningPeriodDto dto, Guid? excludeId)
        {
            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("Startdato skal være før eller samme dag som slutdato.");

            if (await _repository.HasOverlapAsync(dto.StartDate, dto.EndDate, excludeId))
                throw new ArgumentException("Perioden overlapper en eksisterende periode.");
        }

        //CreatedAt kommer ikke med ud til klienten.
        private static PlanningPeriodDto MapToDto(PlanningPeriod period) => new PlanningPeriodDto()
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate
        };
    }
}
