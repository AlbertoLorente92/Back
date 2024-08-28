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
    public class CompanyControllerTest
    {
        private CompanyController _controller;
        private Mock<ITextEncryptionService> _encryptionService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<CompanyController>>();
            _encryptionService = new Mock<ITextEncryptionService>();

            _controller = new CompanyController(mockLogger.Object, _encryptionService.Object);
        }

        [Test]
        public void GetWeather_ShouldReturnWeatherForecasts()
        {
            // Arrange
            var company = new Company()
            {
                Guid = Guid.NewGuid(),
                Id = 1,
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R"
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<Company>(It.IsAny<string>())).Returns(company);
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(company));

            // Act
            var result = _controller.CreateCompany("encryptedCompany");

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            
            Assert.That(JsonConvert.DeserializeObject<Company>((string)(okResult?.Value!)), Is.InstanceOf<Company>());
        }

    }
}
