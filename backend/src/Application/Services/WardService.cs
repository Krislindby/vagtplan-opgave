using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Forretningslogik for afsnit. Oversætter mellem DTO og entitet og håndhæver opgavens regler.
    public class WardService : IWardService
    {
        private readonly IWardRepository _repository;

        public WardService(IWardRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WardDto>> GetAllAsync()
        {
            var wards = await _repository.GetAllAsync();
            return wards.Select(MapToDto);
        }

        public async Task<WardDto?> GetByIdAsync(Guid id)
        {
            var ward = await _repository.GetByIdAsync(id);
            return ward is null ? null : MapToDto(ward);
        }

        public async Task<WardDto> CreateAsync(SaveWardDto dto)
        {
            var name = dto.Name.Trim();

            if (await _repository.ColorExistsAsync(dto.Color))
                throw new ArgumentException("Farven er allerede i brug af et andet afsnit.");
           
            if (await _repository.NameExistsAsync(name))
                throw new ArgumentException("Der findes allerede et afsnit med det navn.");
            
            var ward = new Ward
            {
                Name = name,
                Color = dto.Color
            };

            await _repository.AddAsync(ward);
            return MapToDto(ward);
        }

        public async Task UpdateAsync(Guid id, SaveWardDto dto)
        {
            var name = dto.Name.Trim();

            var ward = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Afsnittet med id {id} blev ikke fundet.");

            //Afsnittets eget id sendes med, så det ikke tæller som dublet af sig selv.
            if (await _repository.NameExistsAsync(name, id))
                throw new ArgumentException("Der findes allerede et afsnit med det navn.");

            if (await _repository.ColorExistsAsync(dto.Color, id))
                throw new ArgumentException("Farven er allerede i brug af et andet afsnit.");

            ward.Name = name;
            ward.Color = dto.Color;

            await _repository.UpdateAsync(ward);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ward = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Afsnittet med id {id} blev ikke fundet.");

            //Opgavens regel: afsnit med tilknyttede ansættelser må ikke slettes.
            if (await _repository.HasEmploymentsAsync(id))
                throw new ArgumentException("Afsnittet kan ikke slettes, fordi der er tilknyttede ansættelser.");

            await _repository.DeleteAsync(ward);
        }

        //CreatedAt kommer ikke med ud til klienten.
        private static WardDto MapToDto(Ward ward) => new()
        {
            Id = ward.Id,
            Name = ward.Name,
            Color = ward.Color
        };
    }
}