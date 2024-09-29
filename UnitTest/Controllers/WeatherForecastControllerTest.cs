using Back.Controllers;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class WeatherForecastControllerTest
    {
        private WeatherForecastController _controller;
        private Mock<ITextEncryptionService> _encryptionService;
        private Mock<IKeyVaultService> _keyVaultService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<WeatherForecastController>>();
            _encryptionService = new Mock<ITextEncryptionService>();
            _keyVaultService = new Mock<IKeyVaultService>();

            _controller = new WeatherForecastController(mockLogger.Object, _encryptionService.Object, _keyVaultService.Object);
        }

        [Test]
        public void GetWeather_ShouldReturnWeatherForecasts()
        {
            // Act
            var result = _controller.GetWeather();

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            
            Assert.That(okResult?.Value, Is.InstanceOf<IEnumerable<WeatherForecast>>());
        }

        [Test]
        public void GetWeatherForDay_WhenDayIsIncorrect_ShouldReturnBadRequest()
        {
            // Arrange
            var day = 0;

            // Act
            var result = _controller.GetWeatherForDay(day);

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void GetWeatherForDay_WhenDayIncorrect_ShouldReturnWeatherForecasts()
        {
            // Arrange
            var day = 10;

            // Act
            var result = _controller.GetWeatherForDay(day);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;

            Assert.That(okResult?.Value, Is.InstanceOf<WeatherForecast>());
        }

        [Test]
        public void DoesTheWeatherMatch_WhenEncryptedWeatherIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var encryptedWeather = string.Empty;

            // Act
            var result = _controller.DoesTheWeatherMatch(encryptedWeather);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DoesTheWeatherMatch_WhenEncryptedWeatherIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            string? encryptedWeather = null!;

            // Act
            var result = _controller.DoesTheWeatherMatch(encryptedWeather);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DoesTheWeatherMatch_WhenEncryptedWeatherIsIncorrect_AndReturnsException_ShouldReturnBadRequest()
        {
            // Arrange
            string encryptedWeather = "{\"date\":\"2024-08-27\",\"temperatureC\":-6,\"temperatureF\":22,\"summary\":\"Warm\"}";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Throws(new Exception());

            // Act
            var result = _controller.DoesTheWeatherMatch(encryptedWeather);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DoesTheWeatherMatch_WhenEncryptedWeatherIsIncorrect_AndReturnsNull_ShouldReturnBadRequest()
        {
            // Arrange
            string encryptedWeather = "{\"date\":\"2024-08-27\",\"temperatureC\":-6,\"temperatureF\":22,\"summary\":\"Warm\"}";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string)null!);

            // Act
            var result = _controller.DoesTheWeatherMatch(encryptedWeather);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void DoesTheWeatherMatch_WhenEncryptedWeatherIsCorrect_AndIsWellEncrypted_ShouldReturnOkObjectResult()
        {
            // Arrange
            var encryptedWeather = "{\"date\":\"2024-08-27\",\"temperatureC\":-6,\"temperatureF\":22,\"summary\":\"Warm\"}";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(encryptedWeather);

            var response = "{\"IsSuccess\":true}";
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DoesTheWeatherMatch(encryptedWeather);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<DoesTheWeatherMatchResponse>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<DoesTheWeatherMatchResponse>());
        }
    }
}
