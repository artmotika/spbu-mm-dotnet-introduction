using System.Globalization;

namespace web_weather
{
    public record class WeatherInfo
    (
        string ServiceName,
        double TemperatureC,
        double CloudCover,
        double Humidity,
        double? RainAccumulation,
        double? SnowAccumulation,
        double WindDirection,
        double WindSpeed
    )
    {
        public double? TemperatureF => 32 + (TemperatureC * 9.0 / 5.0); 
    }
}
