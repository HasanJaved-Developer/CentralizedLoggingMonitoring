using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Contracts.Models
{
    public class Function
    {
        public int Id { get; set; }
        [MaxLength(120)] public string Code { get; set; } = null!;
        [MaxLength(200)] public string DisplayName { get; set; } = null!;

        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        public ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();
    }
}
