using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    //Modtages fra klienten ved oprettelse og redigering. Id kommer fra URL'en.
    public class SaveEmploymentDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid WardId { get; set; }

        [Required]
        public Guid StaffGroupId { get; set; }

        [Range(0, 37, ErrorMessage = "Timer pr. uge skal være et helt tal mellem 0 og 37.")]
        public int HoursPerWeek { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }
}