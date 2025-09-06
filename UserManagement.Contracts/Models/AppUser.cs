using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Contracts.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [MaxLength(120)] public string UserName { get; set; } = null!;
        [MaxLength(200)] public string Password { get; set; } = null!;   // 🔑 new field
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
