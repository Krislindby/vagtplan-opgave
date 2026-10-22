using Application.Services;
using Domain.DTO.PlanningPeriods;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;

namespace Tests
{
    //Tester forretningsreglerne for perioder uden database.
    //Repositoriet erstattes af en Moq-attrap, så kun reglerne i servicen testes.
    public class PlanningPeriodServiceTests
    {
        private readonly Mock<IPlanningPeriodRepository> _repository = new();
        private readonly PlanningPeriodService _service;

        public PlanningPeriodServiceTests()
        {
            _service = new PlanningPeriodService(_repository.Object);
        }

        //Hjælper, så hver test kun skriver de datoer, den handler om.
        private static SavePlanningPeriodDto Dto(string start, string end) => new()
        {
            Name = "Testperiode",
            StartDate = DateOnly.Parse(start),
            EndDate = DateOnly.Parse(end)
        };

        [Fact]
        public async Task CreateAsync_StartdatoEfterSlutdato_KasterFejl()
        {
            var dto = Dto("2026-10-31", "2026-10-01");

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto));

            Assert.Equal("Startdato skal være før eller samme dag som slutdato.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_SammeStartOgSlutdato_ErTilladt()
        {
            var dto = Dto("2026-10-01", "2026-10-01");

            var result = await _service.CreateAsync(dto);

            Assert.Equal(dto.StartDate, result.StartDate);
        }

        [Fact]
        public async Task CreateAsync_OverlapperEksisterendePeriode_KasterFejl()
        {
            //Repositoriet svarer, at tidsrummet er optaget.
            _repository
                .Setup(r => r.HasOverlapAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), null))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto("2026-10-01", "2026-10-31")));

            Assert.Equal("Perioden overlapper en eksisterende periode.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_GyldigPeriode_GemmesIRepositoriet()
        {
            var dto = Dto("2026-10-01", "2026-10-31");

            await _service.CreateAsync(dto);

            //Verify tjekker, at servicen faktisk bad repositoriet om at gemme.
            _repository.Verify(r => r.AddAsync(It.IsAny<PlanningPeriod>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SenderEgetIdSomExcludeId()
        {
            var id = Guid.NewGuid();
            _repository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(new PlanningPeriod
                {
                    Name = "Oktober",
                    StartDate = new DateOnly(2026, 10, 1),
                    EndDate = new DateOnly(2026, 10, 31)
                });

            await _service.UpdateAsync(id, Dto("2026-10-01", "2026-10-31"));

            //Uden excludeId ville perioden overlappe sig selv og aldrig kunne redigeres.
            _repository.Verify(
                r => r.HasOverlapAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), id),
                Times.Once);
        }
    }
}
