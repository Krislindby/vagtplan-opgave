namespace Domain.DTO
{
    //Sendes ud til klienten ved hentning af afsnit.
    public class WardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}