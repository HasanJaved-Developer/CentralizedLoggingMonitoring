namespace UserManagementApi.Contracts.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
