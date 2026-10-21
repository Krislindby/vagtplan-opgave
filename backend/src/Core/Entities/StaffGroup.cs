using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class StaffGroup : BaseEntity
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}