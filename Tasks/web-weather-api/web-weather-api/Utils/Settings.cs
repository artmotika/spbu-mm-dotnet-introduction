using System;
namespace web_weather.Utils
{
	public class Settings : ISettings
	{
        public double Latitude => double.Parse(GetEnvValue("LAT"));
        public double Longitude => double.Parse(GetEnvValue("LNG"));
        public string StormGlassApiKey => GetEnvValue("StormGlassApiKey");
        public string OpenWeatherMapApiKey => GetEnvValue("OpenWeatherMapApiKey");

        private static string GetEnvValue(string variableName)
            => Environment.GetEnvironmentVariable(variableName) ??
            throw new Exception($"Environment variable {variableName} was not found!");
    }
}

