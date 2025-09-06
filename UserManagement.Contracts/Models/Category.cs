using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Contracts.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(100)] public string Name { get; set; } = null!;
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
