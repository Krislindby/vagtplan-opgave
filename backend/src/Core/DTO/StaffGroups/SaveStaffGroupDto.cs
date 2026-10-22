using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering. Id kommer fra URL'en.
    public class SaveStaffGroupDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string Name { get; set; } = string.Empty;
    }
}