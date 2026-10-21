using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering. Id kommer fra URL'en.
    public class SaveWardDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^#([0-9A-Fa-f]{6})$", ErrorMessage = "Color must be a valid HEX code, e.g. #FF0000.")]
        public string Color { get; set; } = string.Empty;
    }
}