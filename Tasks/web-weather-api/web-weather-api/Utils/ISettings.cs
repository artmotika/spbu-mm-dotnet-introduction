using System;
namespace web_weather.Utils
{
	public interface ISettings
	{
        public double Latitude { get; }
        public double Longitude { get; }
        public string StormGlassApiKey { get; }
        public string OpenWeatherMapApiKey { get; }
    }
}

