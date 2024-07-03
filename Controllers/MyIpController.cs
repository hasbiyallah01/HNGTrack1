using Microsoft.AspNetCore.Mvc;
using MYIP.Client;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MYIP.Controllers
{
    [ApiController]
    [Route("api/myip")]
    public class MyIpController : ControllerBase
    {
        private readonly IpApiClient _ipApiClient;
        private readonly OpenWeatherMapClient _weatherClient;

        public MyIpController(IpApiClient ipApiClient, OpenWeatherMapClient weatherClient)
        {
            _ipApiClient = ipApiClient;
            _weatherClient = weatherClient;
        }

        [HttpGet("hello")]
        public async Task<ActionResult> GetHello([FromQuery] string visitor_name, CancellationToken ct)
        {
            try
            {

                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
                var ipAddressWithoutPort = ipAddress?.Split(':')[0];

                var ipApiResponse = await _ipApiClient.Get(ipAddressWithoutPort, ct);
                var city = ipApiResponse?.city;

                if (string.IsNullOrEmpty(city))
                {
                    return NotFound("City not found for the given IP address.");
                }

                var weatherResponse = await _weatherClient.GetWeatherAsync(city, ct);
                var temperature = weatherResponse.Main.Temp;

                var response = new
                {
                    client_ip = ipAddressWithoutPort,
                    location = city,
                    greeting = $"Hello, {visitor_name}!, the temperature is {temperature} degrees Celsius in {city}"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

