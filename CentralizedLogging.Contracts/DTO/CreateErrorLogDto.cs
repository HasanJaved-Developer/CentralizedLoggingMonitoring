namespace CentralizedLogging.Contracts.DTO
{
    public class CreateErrorLogDto
    {
        public int ApplicationId { get; set; }
        public string Severity { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public string? UserId { get; set; }
        public string? RequestId { get; set; }
    }
}
