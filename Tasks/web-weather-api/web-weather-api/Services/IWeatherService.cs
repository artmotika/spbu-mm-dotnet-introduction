using System;
namespace web_weather.Services
{
	public interface IWeatherService
	{
        string Name { get; }
        public Task<WeatherInfo> GetCurrentWeatherInfo();
	}
}

