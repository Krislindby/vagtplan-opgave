using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering. Id kommer fra URL'en.
    public class SaveWardDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^#([0-9A-Fa-f]{6})$", ErrorMessage = "Farven skal være en gyldig HEX-kode, fx #FF0000.")]
        public string Color { get; set; } = string.Empty;
    }
}