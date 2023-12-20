using System;
using web_weather.Utils;
using web_weather.Services;
using web_weather;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Http.Headers;

namespace webweather.Services
{
	public class OpenWeatherMapWeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;
        private readonly ISettings settings;

        public string Name => "openweathermap.org";

        public OpenWeatherMapWeatherService(HttpClient httpClient, ISettings settings)
		{
            this.httpClient = httpClient;
            this.settings = settings;
        }

        public async Task<WeatherInfo> GetCurrentWeatherInfo()
		{
            var baseUrl = "https://api.openweathermap.org/data/2.5/weather?";

            var parameters = new[] {
                $"lat={settings.Latitude}",
                $"lon={settings.Longitude}",
                $"appid={settings.OpenWeatherMapApiKey}",
                "units=metric"
            };

            var requestUri = baseUrl + string.Join("&", parameters);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("accept", "application/json");

            string responseBody;
            using (var response = await httpClient.SendAsync(request, CancellationToken.None))
            {
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }

            var jsonDocument = JsonDocument.Parse(responseBody);
            var main = jsonDocument.RootElement.GetProperty("main");
            var clouds = jsonDocument.RootElement.GetProperty("clouds");
            var wind = jsonDocument.RootElement.GetProperty("wind");

            return new WeatherInfo(
                ServiceName: Name,
                TemperatureC: main.GetProperty("temp").GetDouble(),
                CloudCover: clouds.GetProperty("all").GetDouble(),
                Humidity: main.GetProperty("humidity").GetDouble(),
                RainAccumulation: jsonDocument.RootElement.TryGetProperty("rain", out var rain) ? rain.GetProperty("1h").GetDouble() : null,
                SnowAccumulation: jsonDocument.RootElement.TryGetProperty("snow", out var snow) ? snow.GetProperty("1h").GetDouble() : null,
                WindDirection: wind.GetProperty("deg").GetDouble(),
                WindSpeed: wind.GetProperty("speed").GetDouble()
                );
        }
    }
}

