using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.PlanningPeriods
{
    //Modtages fra klienten ved oprettelse og redigering af planlægningsperioder
    public class SavePlanningPeriodDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Navn må højst være 100 tegn.")]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public DateOnly StartDate { get; set; }
        
        [Required]
        public DateOnly EndDate { get; set; }
    }
}
