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
            var ward = new Ward
            {
                Name = dto.Name,
                Color = dto.Color
            };

            await _repository.AddAsync(ward);
            return MapToDto(ward);
        }

        public async Task UpdateAsync(Guid id, SaveWardDto dto)
        {
            var ward = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ward with id {id} not found.");

            ward.Name = dto.Name;
            ward.Color = dto.Color;

            await _repository.UpdateAsync(ward);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ward = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Ward with id {id} not found.");

            //Opgavens regel: afsnit med tilknyttede ansættelser må ikke slettes.
            if (await _repository.HasEmploymentsAsync(id))
                throw new ArgumentException("The ward cannot be deleted because it has employments.");

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