using System.Globalization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using web_weather.Services;
using web_weather.Models;

namespace web_weather.Controllers
{
    [ApiController]
    [Route("/api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> logger;
        Dictionary<String, IWeatherService> weatherServices;

        public WeatherController(ILogger<WeatherController> logger,
            IEnumerable<IWeatherService> weatherServices)
        {
            this.logger = logger;
            this.weatherServices = weatherServices.ToDictionary(service => service.Name);
        }

        [HttpGet]
        [Route("/sources")]
        public IActionResult GetServices() => Ok(weatherServices.Keys.ToList());

        [HttpGet]
        public async Task<IActionResult> GetCurrentWeatherInfoFromAllSources()
            => Ok(await Task.WhenAll(weatherServices.Values.Select(ToWeatherResponse)));

        [HttpGet]
        [Route("/{serviceName}")]
        public async Task<IActionResult> GetCurrentWeatherInfo(string serviceName)
        {
            IWeatherService? service;
            if (weatherServices.TryGetValue(serviceName, out service))
            {
                return Ok(await ToWeatherResponse(service));
            }
            else
            {
                return NotFound($"Service {service} is not found");
            }
        }

        private async Task<WeatherResponse> ToWeatherResponse(IWeatherService service)
        {
            try
            {
                var weatherInfo = await service.GetCurrentWeatherInfo();
                return new WeatherResponse(service.Name, weatherInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error get weather info from {service}", service.Name);
                return new WeatherResponse(service.Name, null);
            }
        }
    }
}
