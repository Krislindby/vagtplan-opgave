namespace Domain.DTO
{
    //Sendes ud til klienten. Indeholder både id'er til dropdowns og navne til visning i tabellen.
    public class EmploymentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public Guid WardId { get; set; }
        public string WardName { get; set; } = string.Empty;

        public Guid StaffGroupId { get; set; }
        public string StaffGroupName { get; set; } = string.Empty;

        public int HoursPerWeek { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}