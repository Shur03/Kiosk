using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using KioskApp.Services.DTOs;

namespace KioskApp.Services.Api
{
    public class NumberSearchService
    {
        private const string SearchUrl = "https://new.skytel.mn/number/search";
        private static readonly HttpClient HttpClient = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public async Task<NumberSearchResponse> SearchAsync(NumberSearchRequest request)
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["type"] = request.Type,
                ["searchType"] = request.SearchType,
                ["number"] = request.Number,
                ["page"] = request.Page.ToString(),
                ["limit"] = request.Limit.ToString(),
            });

            using var response = await HttpClient.PostAsync(SearchUrl, form);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NumberSearchResponse>(json, JsonOptions);

            if (result == null)
                throw new InvalidOperationException("Empty response from number search API.");

            if (result.ResultCode != "1000")
                throw new InvalidOperationException(result.ResultMsg ?? "Number search failed.");

            result.Numbers ??= new List<AvailableNumberDto>();
            return result;
        }
    }
}
