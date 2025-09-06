using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedLogging.Contracts.Models
{
    public class GetAllErrorsResponseModel
    {
        public long Id { get; set; }
        public string Severity { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public string? UserId { get; set; }
        public string? RequestId { get; set; }
        public DateTime LoggedAt { get; set; }

        // Instead of ApplicationId, expose the name
        public string ApplicationName { get; set; } = null!;
    }
}
