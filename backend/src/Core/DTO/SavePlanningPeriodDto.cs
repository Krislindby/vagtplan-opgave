using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering af planlægningsperioder
    public class SavePlanningPeriodDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public DateOnly StartDate { get; set; }
        
        [Required]
        public DateOnly EndDate { get; set; }
    }
}
