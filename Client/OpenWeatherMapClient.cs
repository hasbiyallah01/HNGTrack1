using System.Text.Json;

namespace MYIP.Client
{
    public class OpenWeatherMapClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city, CancellationToken ct)
        {
            var response = await _httpClient.GetAsync(
                $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric",
                ct);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<WeatherResponse>(content);
        }
    }

    public class WeatherResponse
    {
        public MainInfo Main { get; set; }
    }

    public class MainInfo
    {
        public float Temp { get; set; }
    }
}

