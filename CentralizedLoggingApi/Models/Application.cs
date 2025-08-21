using System.Text.Json.Serialization;

namespace CentralizedLoggingApi.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string Environment { get; set; } = "Production";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();
    }
}
