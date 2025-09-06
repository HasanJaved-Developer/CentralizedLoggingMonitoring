using System.Net.Http.Json;
using System.Text.Json;
using CentralizedLogging.Contracts.Models;
using CentralizedLogging.Sdk.Abstractions;
using Microsoft.AspNetCore.Mvc;

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
    }
}
