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

namespace test_web_weather
{
    [TestClass]
    public class TestWeatherController
    {
        private Mock<IWeatherService>? service1;
        private Mock<IWeatherService>? service2;
        private WeatherController? controller;

        [TestInitialize]
        public void Init()
        {
            service1 = new Mock<IWeatherService>();
            service2 = new Mock<IWeatherService>();

            service1.Setup(s => s.GetCurrentWeatherInfo()).ReturnsAsync(new WeatherInfo(
                    ServiceName: "Service1",
                    TemperatureC: 23.0,
                    CloudCover: 10.0,
                    Humidity: 50.0,
                    RainAccumulation: 2.0,
                    SnowAccumulation: 0.0,
                    WindDirection: 1.0,
                    WindSpeed: 1.0
                ));
            service1.Setup(s => s.Name).Returns("Service1");

            service2.Setup(s => s.GetCurrentWeatherInfo()).ThrowsAsync(new System.Exception("Service error!"));
            service2.Setup(s => s.Name).Returns("Service2");

            Mock<ILogger<WeatherController>> loggerMock
                = new Mock<ILogger<WeatherController>>();
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            List<IWeatherService> weatherServices
                = new List<IWeatherService> { service1.Object, service2.Object };
            controller
                = new WeatherController(loggerMock.Object, weatherServices);
        }

        [TestMethod]
        public void TestGetServices()
        {
            Assert.IsNotNull(controller);
            var result = controller.GetServices();
            var okObjectResult = (OkObjectResult)result;
            var actualResult = okObjectResult.Value as List<string>;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(2, actualResult.Count);
            Assert.IsTrue(actualResult.Contains("Service1"));
            Assert.IsTrue(actualResult.Contains("Service2"));
        }

        [TestMethod]
        public async Task GetCurrentWeatherInfoFromAllSources()
        {
            Assert.IsNotNull(controller);
            var result = await controller.GetCurrentWeatherInfoFromAllSources(); 
            var okObjectResult = (OkObjectResult)result;
            var actualResult = okObjectResult.Value as IEnumerable<WeatherResponse>;
            Assert.IsNotNull(actualResult);
            var weatherResponse1 = actualResult.First(r => r.ServiceName == "Service1");
            Assert.AreEqual("23", weatherResponse1.TemperatureC);
            Assert.AreEqual("73,4", weatherResponse1.TemperatureF);
            Assert.AreEqual("10", weatherResponse1.CloudCover);
            Assert.AreEqual("50", weatherResponse1.Humidity);
            Assert.AreEqual("2", weatherResponse1.RainAccumulation);
            Assert.AreEqual("0", weatherResponse1.SnowAccumulation);
            Assert.AreEqual("1", weatherResponse1.WindDirection);
            Assert.AreEqual("1", weatherResponse1.WindSpeed);

            var weatherResponse2 = actualResult.First(r => r.ServiceName == "Service2");
            Assert.AreEqual("Нет данных!", weatherResponse2.TemperatureC);
            Assert.AreEqual("Нет данных!", weatherResponse2.TemperatureF);
            Assert.AreEqual("Нет данных!", weatherResponse2.CloudCover);
            Assert.AreEqual("Нет данных!", weatherResponse2.Humidity);
            Assert.AreEqual("Нет данных!", weatherResponse2.RainAccumulation);
            Assert.AreEqual("Нет данных!", weatherResponse2.SnowAccumulation);
            Assert.AreEqual("Нет данных!", weatherResponse2.WindDirection);
            Assert.AreEqual("Нет данных!", weatherResponse2.WindSpeed);
        }

        [TestMethod]
        public async Task GetCurrentWeatherInfoFromService1()
        {
            Assert.IsNotNull(controller);
            var result = await controller.GetCurrentWeatherInfo("Service1");
            var okObjectResult = (OkObjectResult)result;
            var actualResult = okObjectResult.Value as WeatherResponse;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual("Service1", actualResult.ServiceName);
            Assert.AreEqual("23", actualResult.TemperatureC);
            Assert.AreEqual("73,4", actualResult.TemperatureF);
            Assert.AreEqual("10", actualResult.CloudCover);
            Assert.AreEqual("50", actualResult.Humidity);
            Assert.AreEqual("2", actualResult.RainAccumulation);
            Assert.AreEqual("0", actualResult.SnowAccumulation);
            Assert.AreEqual("1", actualResult.WindDirection);
            Assert.AreEqual("1", actualResult.WindSpeed);
        }

        [TestMethod]
        public async Task GetCurrentWeatherInfoFromService2()
        {
            Assert.IsNotNull(controller);
            var result = await controller.GetCurrentWeatherInfo("Service2");
            var okObjectResult = (OkObjectResult)result;
            var actualResult = okObjectResult.Value as WeatherResponse;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual("Нет данных!", actualResult.TemperatureC);
            Assert.AreEqual("Нет данных!", actualResult.TemperatureF);
            Assert.AreEqual("Нет данных!", actualResult.CloudCover);
            Assert.AreEqual("Нет данных!", actualResult.Humidity);
            Assert.AreEqual("Нет данных!", actualResult.RainAccumulation);
            Assert.AreEqual("Нет данных!", actualResult.SnowAccumulation);
            Assert.AreEqual("Нет данных!", actualResult.WindDirection);
            Assert.AreEqual("Нет данных!", actualResult.WindSpeed);
        }
    }
}
