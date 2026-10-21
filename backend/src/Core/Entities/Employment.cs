using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Employment : BaseEntity
    {
        [Range(0, 37, ErrorMessage = "Hours per week must be a whole number between 0 and 37.")]
        public int HoursPerWeek { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        //FK refererer til den person, som denne ansættelse tilhører.
        public Guid PersonId { get; set; }
        public Person? Person { get; set; }

        //FK refererer til det afsnit, som ansættelsen er knyttet til.
        public Guid WardId { get; set; }
        public Ward? Ward { get; set; }

        //FK refererer til den personalegruppe, som personen tilhører i denne ansættelse.
        public Guid StaffGroupId { get; set; }
        public StaffGroup? StaffGroup { get; set; }
    }
}