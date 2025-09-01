namespace UserManagementApi.Models
{
    public class RoleFunction
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public int FunctionId { get; set; }
        public Function Function { get; set; } = null!;
    }
}
