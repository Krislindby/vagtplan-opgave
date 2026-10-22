using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Application.Services
{
    //Forretningslogik for adgange. Oversætter mellem DTO og entitet og håndhæver opgavens regler.
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user is null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateAsync(SaveUserDto dto)
        {
            await ValidateAsync(dto, null);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                IsAdmin = dto.IsAdmin,
                IsStaff = dto.IsStaff
            };

            await _repository.AddAsync(user);
            return MapToDto(user);
        }

        public async Task UpdateAsync(Guid id, SaveUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Brugeren med id {id} blev ikke fundet.");

            //Brugerens eget id sendes med, så e-mailen ikke tæller som dublet af sig selv.
            await ValidateAsync(dto, id);

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.IsAdmin = dto.IsAdmin;
            user.IsStaff = dto.IsStaff;

            await _repository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Brugeren med id {id} blev ikke fundet.");

            //Opgavens regel: brugere med ansættelser må ikke slettes.
            if (await _repository.HasEmploymentsAsync(id))
                throw new ArgumentException("Brugeren kan ikke slettes, fordi der er tilknyttede ansættelser.");

            await _repository.DeleteAsync(user);
        }

        //Opgavens to regler, samlet ét sted så opret og redigér bruger de samme.
        private async Task ValidateAsync(SaveUserDto dto, Guid? excludeId)
        {
            if (!dto.IsAdmin && !dto.IsStaff)
                throw new ArgumentException("Brugeren skal have mindst én rolle.");

            if (await _repository.EmailExistsAsync(dto.Email, excludeId))
                throw new ArgumentException("E-mailen er allerede i brug.");
        }

        //CreatedAt kommer ikke med ud til klienten.
        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsAdmin = user.IsAdmin,
            IsStaff = user.IsStaff
        };
    }
}