namespace Domain.DTO
{
    //Sendes ud til klienten ved hentning af personalegrupper.
    public class StaffGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}