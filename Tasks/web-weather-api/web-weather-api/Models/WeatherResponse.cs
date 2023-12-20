using System;
using System.Globalization;

namespace web_weather.Models
{
	public class WeatherResponse
    {
        public string ServiceName { get; }
        public string TemperatureC { get; }
        public string TemperatureF { get; }
        public string CloudCover { get; }
        public string Humidity { get; }
        public string RainAccumulation { get; }
        public string SnowAccumulation { get; }
        public string WindDirection { get; }
        public string WindSpeed { get; }

        public WeatherResponse(string serviceName, WeatherInfo? weatherInfo)
        {
            ServiceName = serviceName;
            TemperatureC = Format(weatherInfo?.TemperatureC);
            TemperatureF = Format(weatherInfo?.TemperatureF);
            CloudCover = Format(weatherInfo?.CloudCover);
            Humidity = Format(weatherInfo?.Humidity);
            RainAccumulation = Format(weatherInfo?.RainAccumulation);
            SnowAccumulation = Format(weatherInfo?.SnowAccumulation);
            WindDirection = Format(weatherInfo?.WindDirection);
            WindSpeed = Format(weatherInfo?.WindSpeed);
        }

        private static string Format(double? data)
            => data?.ToString() ?? "Нет данных!";
    }
}

