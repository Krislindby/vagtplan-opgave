namespace Domain.DTO
{
    //Sendes ud til klienten ved hentning af adgange.
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsStaff { get; set; }
    }
}