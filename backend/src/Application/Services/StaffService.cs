using Domain.DTO;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Finder det personale, hvis ansættelse overlapper en valgt periode.
    public class StaffService : IStaffService
    {
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IPlanningPeriodRepository _periodRepository;

        public StaffService(
            IEmploymentRepository employmentRepository,
            IPlanningPeriodRepository periodRepository)
        {
            _employmentRepository = employmentRepository;
            _periodRepository = periodRepository;
        }

        public async Task<IEnumerable<StaffDto>> GetByPeriodAsync(Guid planningPeriodId)
        {
            var period = await _periodRepository.GetByIdAsync(planningPeriodId)
                ?? throw new KeyNotFoundException($"Perioden med id {planningPeriodId} blev ikke fundet.");

            var employments = await _employmentRepository.GetOverlappingAsync(
                period.StartDate, period.EndDate);

            return employments.Select(e => new StaffDto
            {
                Id = e.Id,
                Name = e.User!.Name,
                WardName = e.Ward!.Name,
                StaffGroupName = e.StaffGroup!.Name,
                HoursPerWeek = e.HoursPerWeek,
            });
        }
    }
}