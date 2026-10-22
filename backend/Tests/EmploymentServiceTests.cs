using Application.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;

namespace Tests
{
    //Tester forretningsreglerne for ansættelser uden database.
    //Alle fire repositories erstattes af Moq-attrapper, så kun reglerne i servicen testes.
    public class EmploymentServiceTests
    {
        private readonly Mock<IEmploymentRepository> _repository = new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IWardRepository> _wardRepository = new();
        private readonly Mock<IStaffGroupRepository> _staffGroupRepository = new();
        private readonly EmploymentService _service;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _wardId = Guid.NewGuid();
        private readonly Guid _staffGroupId = Guid.NewGuid();

        public EmploymentServiceTests()
        {
            _service = new EmploymentService(
                _repository.Object,
                _userRepository.Object,
                _wardRepository.Object,
                _staffGroupRepository.Object);

            //Udgangspunktet er, at alt findes og personen er personale.
            //Den enkelte test ændrer kun det, den handler om.
            _userRepository
                .Setup(r => r.GetByIdAsync(_userId))
                .ReturnsAsync(new User { Name = "Emil Hansen", Email = "emil@hospital.dk", IsStaff = true });

            _wardRepository
                .Setup(r => r.GetByIdAsync(_wardId))
                .ReturnsAsync(new Ward { Name = "Skadestue", Color = "#e5484d" });

            _staffGroupRepository
                .Setup(r => r.GetByIdAsync(_staffGroupId))
                .ReturnsAsync(new StaffGroup { Name = "Overlæge" });

            //CreateAsync henter ansættelsen igen for at få navnene med i svaret.
            _repository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Employment
                {
                    HoursPerWeek = 37,
                    StartDate = new DateOnly(2026, 1, 1),
                    EndDate = new DateOnly(2026, 12, 31),
                    User = new User { Name = "Emil Hansen", Email = "emil@hospital.dk", IsStaff = true },
                    Ward = new Ward { Name = "Skadestue", Color = "#e5484d" },
                    StaffGroup = new StaffGroup { Name = "Overlæge" }
                });
        }

        //Hjælper, så hver test kun skriver det, den handler om.
        private SaveEmploymentDto Dto(string start = "2026-01-01", string end = "2026-12-31") => new()
        {
            UserId = _userId,
            WardId = _wardId,
            StaffGroupId = _staffGroupId,
            HoursPerWeek = 37,
            StartDate = DateOnly.Parse(start),
            EndDate = DateOnly.Parse(end)
        };

        [Fact]
        public async Task CreateAsync_SlutdatoFoerStartdato_KasterFejl()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto("2026-12-31", "2026-01-01")));

            Assert.Equal("Slutdato skal være efter startdato.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_SammeStartOgSlutdato_KasterFejl()
        {
            //En ansættelse på nul dage giver ikke mening, så datoerne må ikke være ens.
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto("2026-01-01", "2026-01-01")));

            Assert.Equal("Slutdato skal være efter startdato.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_BrugerFindesIkke_KasterFejl()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(_userId))
                .ReturnsAsync((User?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Den valgte bruger findes ikke.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_BrugerUdenRollenPersonale_KasterFejl()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(_userId))
                .ReturnsAsync(new User { Name = "Mette Holm", Email = "mette@hospital.dk", IsAdmin = true, IsStaff = false });

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Den valgte bruger skal have rollen Personale.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_AfsnitFindesIkke_KasterFejl()
        {
            _wardRepository
                .Setup(r => r.GetByIdAsync(_wardId))
                .ReturnsAsync((Ward?)null);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Det valgte afsnit findes ikke.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_OverlappendeAnsaettelse_KasterFejl()
        {
            _repository
                .Setup(r => r.HasOverlapAsync(_userId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), null))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Brugeren har allerede en ansættelse i denne periode.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_GyldigAnsaettelse_GemmesIRepositoriet()
        {
            await _service.CreateAsync(Dto());

            _repository.Verify(r => r.AddAsync(It.IsAny<Employment>()), Times.Once);
        }
    }
}
