using System;
using System.Globalization;
using System.Text.Json;
using System.Web;
using web_weather.Utils;
using web_weather.Services;
using web_weather;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace webweather.Services
{
	public class StormGlassWeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;
        private readonly ISettings settings;

        public string Name => "stormglass.io";

        public StormGlassWeatherService(HttpClient httpClient, ISettings settings)
		{
            this.httpClient = httpClient;
            this.settings = settings;
        }

        public async Task<WeatherInfo> GetCurrentWeatherInfo()
        {
            var baseUrl = "https://api.stormglass.io/v2/weather/point?";

            var metrics = new[]
            {
                "airTemperature",
                "cloudCover",
                "humidity",
                "precipitation",
                "windSpeed",
                "windDirection"
            };

            var date = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

            var parameters = new[] {
                $"lat={settings.Latitude}",
                $"lng={settings.Longitude}",
                $"params={string.Join(",", metrics)}",
                $"start={date}",
                $"end={date}",
                "source=noaa"
            };

            var requestUri = baseUrl + string.Join("&", parameters);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("authorization", settings.StormGlassApiKey);

            string responseBody;
            using (var response = await httpClient.SendAsync(request, CancellationToken.None))
            {
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }

            var jsonDocument = JsonDocument.Parse(responseBody);
            var hours = jsonDocument.RootElement.GetProperty("hours").EnumerateArray().First();

            return new WeatherInfo(
                ServiceName: Name,
                TemperatureC: hours.GetProperty("airTemperature").GetProperty("noaa").GetDouble(),
                CloudCover: hours.GetProperty("cloudCover").GetProperty("noaa").GetDouble(),
                Humidity: hours.GetProperty("humidity").GetProperty("noaa").GetDouble(),
                RainAccumulation: hours.GetProperty("precipitation").GetProperty("noaa").GetDouble(),
                SnowAccumulation: null,
                WindDirection: hours.GetProperty("windDirection").GetProperty("noaa").GetDouble(),
                WindSpeed: hours.GetProperty("windSpeed").GetProperty("noaa").GetDouble()
                );
        }
    }
}

