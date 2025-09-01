using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models
{
    public class Module
    {
        public int Id { get; set; }
        [MaxLength(100)] public string Name { get; set; } = null!;
        [MaxLength(100)] public string Area { get; set; } = null!;
        [MaxLength(100)] public string Controller { get; set; } = null!;
        [MaxLength(100)] public string Action { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<Function> Functions { get; set; } = new List<Function>();
    }
}
