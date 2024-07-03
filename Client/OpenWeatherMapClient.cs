using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

public class OpenWeatherMapClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMapApiKey"];
    }

    public async Task<WeatherResponse> GetWeatherAsync(string city, CancellationToken ct)
    {
        var apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

        var response = await _httpClient.GetAsync(apiUrl, ct);
        response.EnsureSuccessStatusCode(); // Ensure HTTP success status code

        var content = await response.Content.ReadAsStringAsync(ct);
        Console.WriteLine("Response Content: " + content); // Verify the content received

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Ensure case insensitivity
            IgnoreNullValues = true // Ignore null values during deserialization
        };

        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(content, options);

        if (weatherResponse == null)
        {
            throw new Exception("Failed to deserialize JSON response");
        }

        // Access weather data
        var temperature = weatherResponse.Main.Temp;
        var cityName = weatherResponse.Name;

        Console.WriteLine($"Temperature in {cityName}: {temperature}°C");


        if (weatherResponse == null || weatherResponse.Main == null)
        {
            throw new Exception("Weather response or MainInfo is null");
        }

        return weatherResponse;
    }

}

public class WeatherResponse
{
    public Coord Coord { get; set; }
    public WeatherInfo[] Weather { get; set; }
    public string Base { get; set; }
    public MainInfo Main { get; set; }
    public int Visibility { get; set; }
    public Wind Wind { get; set; }
    public Rain Rain { get; set; }
    public Clouds Clouds { get; set; }
    public long Dt { get; set; }
    public Sys Sys { get; set; }
    public int Timezone { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cod { get; set; }
}

public class Coord
{
    public float Lon { get; set; }
    public float Lat { get; set; }
}

public class WeatherInfo
{
    public int Id { get; set; }
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class MainInfo
{
    public float Temp { get; set; }
    public float FeelsLike { get; set; }
    public float TempMin { get; set; }
    public float TempMax { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public int SeaLevel { get; set; }
    public int GrndLevel { get; set; }
}

public class Wind
{
    public float Speed { get; set; }
    public int Deg { get; set; }
    public float Gust { get; set; }
}

public class Rain
{
    [JsonPropertyName("1h")]
    public float OneHour { get; set; }
}

public class Clouds
{
    public int All { get; set; }
}

public class Sys
{
    public string Country { get; set; }
    public long Sunrise { get; set; }
    public long Sunset { get; set; }
}
