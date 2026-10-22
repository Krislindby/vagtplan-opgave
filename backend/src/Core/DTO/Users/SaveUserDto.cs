using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering. Id kommer fra URL'en.
    public class SaveUserDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "E-mail skal være en gyldig e-mailadresse.")]
        [StringLength(200, ErrorMessage = "E-mail må højst være 200 tegn.")]
        public string Email { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public bool IsStaff { get; set; }
    }
}