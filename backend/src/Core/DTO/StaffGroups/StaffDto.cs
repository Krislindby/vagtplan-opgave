namespace Domain.DTO
{
    //Skrivebeskyttet visning af personale i en valgt periode. Id er ansættelsens, så tabellen har en nøgle.
    public class StaffDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string WardName { get; set; } = string.Empty;
        public string StaffGroupName { get; set; } = string.Empty;
        public int HoursPerWeek { get; set; }
    }
}