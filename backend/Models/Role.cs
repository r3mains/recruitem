using System.ComponentModel.DataAnnotations;

namespace recruitem_backend.Models
{
    public class Role
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = "";

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
