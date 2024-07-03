using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MYIP.Client
{
    public class IpApiClient
    {
        private const string BASE_URL = "http://ip-api.com";
        private readonly HttpClient _httpClient;

        public IpApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IpApiResponse?> Get(string? ipAddress, CancellationToken ct)
        {
            var route = $"{BASE_URL}/json/{ipAddress}";
            var response = await _httpClient.GetAsync(route, ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching IP info: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<IpApiResponse>(content);
        }
    }

    public class IpApiResponse
    {
        public string? city { get; set; }
        public string? country { get; set; }
        public string? regionName { get; set; }
        // Add other properties as needed
    }
}


