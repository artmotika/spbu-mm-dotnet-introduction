using Moq;
using web_weather.Utils;
using web_weather.Services;
using web_weather.Models;
using System.Threading.Tasks;
using web_weather.Controllers;
using System.Text.Json;
using System.Xml.Linq;
using web_weather;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webweather.Services;
using System.Net.Http;


namespace test_web_weather
{
    [TestClass]
    public class TestWeatherServices
    {
        private Mock<HttpClient>? httpClient;
        private Mock<ISettings>? settings;
        private OpenWeatherMapWeatherService? openWeatherMapWeatherService;
        private StormGlassWeatherService? stormGlassWeatherService;

        [TestInitialize]
        public void Init()
        {
            httpClient = new Mock<HttpClient>();
            settings = new Mock<ISettings>();

            settings.Setup(env => env.Latitude).Returns(59.93863);
            settings.Setup(env => env.Longitude).Returns(30.31413);
            settings.Setup(env => env.OpenWeatherMapApiKey).Returns("123");
            settings.Setup(env => env.StormGlassApiKey).Returns("123");

            Assert.IsNotNull(httpClient);
            Assert.IsNotNull(settings);
            openWeatherMapWeatherService
                = new OpenWeatherMapWeatherService(
                    httpClient.Object,
                    settings.Object);
            stormGlassWeatherService
                = new StormGlassWeatherService(
                    httpClient.Object,
                    settings.Object);
        }

        [TestMethod]
        public async Task OpenWeatherMapWeatherServiceGetCurrentWeatherInfo()
        {

            Assert.IsNotNull(httpClient);
            Assert.IsNotNull(openWeatherMapWeatherService);
            var jsonResponse = @"
            {
              ""coord"": {
                ""lon"": 30.3141,
                ""lat"": 59.9386
              },
              ""weather"": [
                {
                  ""id"": 804,
                  ""main"": ""Clouds"",
                  ""description"": ""overcast clouds"",
                  ""icon"": ""04n""
                }
              ],
              ""base"": ""stations"",
              ""main"": {
                ""temp"": -10.0,
                ""feels_like"": -8.59,
                ""temp_min"": -7.32,
                ""temp_max"": -3.95,
                ""pressure"": 1018,
                ""humidity"": 93
              },
              ""visibility"": 10000,
              ""wind"": {
                ""speed"": 3,
                ""deg"": 280
              },
              ""clouds"": {
                ""all"": 100
              },
              ""dt"": 1702698761,
              ""sys"": {
                ""type"": 1,
                ""id"": 8926,
                ""country"": ""RU"",
                ""sunrise"": 1702709756,
                ""sunset"": 1702731146
              },
              ""timezone"": 10800,
              ""id"": 519690,
              ""name"": ""Novaya Gollandiya"",
              ""cod"": 200
            }";

            httpClient.Setup(
               client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(jsonResponse),
            });

            var weatherInfo = await openWeatherMapWeatherService.GetCurrentWeatherInfo();
            Assert.IsNotNull(weatherInfo);
            Assert.AreEqual("openweathermap.org", weatherInfo.ServiceName);
            Assert.AreEqual(-10, weatherInfo.TemperatureC);
            Assert.AreEqual(14, weatherInfo.TemperatureF);
            Assert.AreEqual(100, weatherInfo.CloudCover);
            Assert.AreEqual(93, weatherInfo.Humidity);
            Assert.AreEqual(null, weatherInfo.RainAccumulation);
            Assert.AreEqual(null, weatherInfo.SnowAccumulation);
            Assert.AreEqual(280, weatherInfo.WindDirection);
            Assert.AreEqual(3, weatherInfo.WindSpeed);
        }

        [TestMethod]
        public async Task StormGlassWeatherServiceGetCurrentWeatherInfo()
        {
            Assert.IsNotNull(httpClient);
            Assert.IsNotNull(stormGlassWeatherService);
            var jsonResponse = @"
            {
              ""hours"": [
                {
                  ""airTemperature"": {
                    ""noaa"": -10.0
                  },
                  ""cloudCover"": {
                    ""noaa"": 89.8
                  },
                  ""humidity"": {
                    ""noaa"": 97.37
                  },
                  ""precipitation"": {
                    ""noaa"": 0.01
                  },
                  ""time"": ""2023-12-15T23:00:00+00:00"",
                  ""windDirection"": {
                    ""noaa"": 293.99
                  },
                  ""windSpeed"": {
                    ""noaa"": 2.02
                  }
                }
              ],
              ""meta"": {
                ""cost"": 1,
                ""dailyQuota"": 10,
                ""end"": ""2023-12-15 23:30"",
                ""lat"": 59.93863,
                ""lng"": 30.31413,
                ""params"": [
                  ""airTemperature"",
                  ""cloudCover"",
                  ""humidity"",
                  ""precipitation"",
                  ""windSpeed"",
                  ""windDirection""
                ],
                ""requestCount"": 1,
                ""source"": [
                  ""noaa""
                ],
                ""start"": ""2023-12-15 23:00""
              }
            }";

            httpClient.Setup(
               client => client.SendAsync(
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
               )
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(jsonResponse),
            });

            var weatherInfo = await stormGlassWeatherService.GetCurrentWeatherInfo();
            Assert.IsNotNull(weatherInfo);
            Assert.AreEqual("stormglass.io", weatherInfo.ServiceName);
            Assert.AreEqual(-10, weatherInfo.TemperatureC);
            Assert.AreEqual(14, weatherInfo.TemperatureF);
            Assert.AreEqual(89.8, weatherInfo.CloudCover);
            Assert.AreEqual(97.37, weatherInfo.Humidity);
            Assert.AreEqual(0.01, weatherInfo.RainAccumulation);
            Assert.AreEqual(null, weatherInfo.SnowAccumulation);
            Assert.AreEqual(293.99, weatherInfo.WindDirection);
            Assert.AreEqual(2.02, weatherInfo.WindSpeed);
        }
    }
}
