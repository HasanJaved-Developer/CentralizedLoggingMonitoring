using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Contracts.Models
{
    public class Role
    {
        public int Id { get; set; }
        [MaxLength(100)] public string Name { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();
    }
}
