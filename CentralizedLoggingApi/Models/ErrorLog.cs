using System.Text.Json.Serialization;

namespace CentralizedLoggingApi.Models
{
    public class ErrorLog
    {
        public long Id { get; set; }
        public int ApplicationId { get; set; }
        public string Severity { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public string? UserId { get; set; }
        public string? RequestId { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore] 
        public Application Application { get; set; } = null!;


    }
}
