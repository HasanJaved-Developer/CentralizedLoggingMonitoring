using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        [MaxLength(120)] public string UserName { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
