using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PlanningPeriod : BaseEntity
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
