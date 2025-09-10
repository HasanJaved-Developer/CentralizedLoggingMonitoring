using CentralizedLogging.Contracts.DTO;
using CentralizedLogging.Contracts.Models;
using CentralizedLogging.Sdk.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace CentralizedLogging.Sdk
{
    internal sealed class CentralizedLoggingClient : ICentralizedLoggingClient
    {
        private readonly HttpClient _http;

        public CentralizedLoggingClient(HttpClient http) => _http = http;

        public async Task<List<GetAllErrorsResponseModel>> GetAllErrorAsync(CancellationToken ct = default)
        {
            var resp = await _http.GetFromJsonAsync<List<GetAllErrorsResponseModel>>("api/errorlogs");
            return resp;
        }

        public async Task LogErrorAsync(CreateErrorLogDto request, CancellationToken ct)
        {            
            var resp = await _http.PostAsJsonAsync("api/errorlogs", request, ct);            
        }
    }
}
