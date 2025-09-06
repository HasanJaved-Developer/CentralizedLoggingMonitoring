using CentralizedLogging.Contracts.Models;
namespace CentralizedLogging.Sdk.Abstractions
{
    public interface ICentralizedLoggingClient
    {
        Task<List<GetAllErrorsResponseModel>> GetAllErrorAsync(CancellationToken ct = default);
    }
}
