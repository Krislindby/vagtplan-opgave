using Application.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;

namespace Tests
{
    //Tester forretningsreglerne for afsnit uden database.
    //Repositoriet erstattes af en Moq-attrap, så kun reglerne i servicen testes.
    public class WardServiceTests
    {
        private readonly Mock<IWardRepository> _repository = new();
        private readonly WardService _service;

        public WardServiceTests()
        {
            _service = new WardService(_repository.Object);
        }

        private static SaveWardDto Dto(string name = "Neurologi", string color = "#0091ff") => new()
        {
            Name = name,
            Color = color
        };

        [Fact]
        public async Task CreateAsync_NavnetErOptaget_KasterFejl()
        {
            _repository
                .Setup(r => r.NameExistsAsync(It.IsAny<string>(), null))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Der findes allerede et afsnit med det navn.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_FarvenErOptaget_KasterFejl()
        {
            _repository
                .Setup(r => r.ColorExistsAsync(It.IsAny<string>(), null))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(Dto()));

            Assert.Equal("Farven er allerede i brug af et andet afsnit.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_MellemrumOmkringNavnet_Fjernes()
        {
            //Uden Trim ville " Neurologi " og "Neurologi" tælle som to forskellige afsnit.
            await _service.CreateAsync(Dto("  Neurologi  "));

            _repository.Verify(
                r => r.AddAsync(It.Is<Ward>(w => w.Name == "Neurologi")),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_GyldigtAfsnit_GemmesIRepositoriet()
        {
            await _service.CreateAsync(Dto());

            _repository.Verify(r => r.AddAsync(It.IsAny<Ward>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_AfsnitMedAnsaettelser_KasterFejl()
        {
            var id = Guid.NewGuid();

            _repository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(new Ward { Name = "Skadestue", Color = "#e5484d" });

            _repository
                .Setup(r => r.HasEmploymentsAsync(id))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.DeleteAsync(id));

            Assert.Equal("Afsnittet kan ikke slettes, fordi der er tilknyttede ansættelser.", ex.Message);
            _repository.Verify(r => r.DeleteAsync(It.IsAny<Ward>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_AfsnitUdenAnsaettelser_Slettes()
        {
            var id = Guid.NewGuid();

            _repository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(new Ward { Name = "Skadestue", Color = "#e5484d" });

            await _service.DeleteAsync(id);

            _repository.Verify(r => r.DeleteAsync(It.IsAny<Ward>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_AfsnittetFindesIkke_KasterFejl()
        {
            _repository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Ward?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_SenderEgetIdSomExcludeId()
        {
            var id = Guid.NewGuid();

            _repository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(new Ward { Name = "Neurologi", Color = "#0091ff" });

            await _service.UpdateAsync(id, Dto());

            //Uden excludeId ville afsnittet tælle som dublet af sig selv og aldrig kunne gemmes.
            _repository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), id), Times.Once);
            _repository.Verify(r => r.ColorExistsAsync(It.IsAny<string>(), id), Times.Once);
        }
    }
}
