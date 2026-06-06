using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KioskApp.Services.DTOs
{
    public class NumberSearchRequest
    {
        public string Type { get; set; } = "hybrid";
        public string SearchType { get; set; } = "all";
        public string Number { get; set; } = "69******";
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 108;
    }

    public class NumberSearchResponse
    {
        [JsonPropertyName("result_code")]
        public string? ResultCode { get; set; }

        [JsonPropertyName("result_msg")]
        public string? ResultMsg { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("numbers")]
        public List<AvailableNumberDto>? Numbers { get; set; }

        [JsonPropertyName("totalRows")]
        public string? TotalRows { get; set; }
    }

    public class AvailableNumberDto
    {
        [JsonPropertyName("PhoneId")]
        public string? PhoneId { get; set; }

        [JsonPropertyName("PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("PhoneType")]
        public string? PhoneType { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("priceType")]
        public string? PriceType { get; set; }
    }
}
