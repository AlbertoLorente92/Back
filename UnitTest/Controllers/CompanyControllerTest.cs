using Back.Controllers;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private ITextEncryptionService _realEncryptionService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<CompanyController>>();
            _encryptionService = new Mock<ITextEncryptionService>();

            var mockSectionCompaniesFile = new Mock<IConfigurationSection>();
            mockSectionCompaniesFile.Setup(x => x.Value).Returns("FilesDB\\Companies.txt");

            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAAAAAAAAAA");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");

            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("CompaniesFile")).Returns(mockSectionCompaniesFile.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);

            _realEncryptionService = new AesEncryptionService(_mockConfiguration.Object);

            _controller = new CompanyController(mockLogger.Object, _encryptionService.Object, _mockConfiguration.Object);
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

            var serializeCompany = JsonConvert.SerializeObject(company);

            var encryptedCompany = _realEncryptionService.Encrypt(serializeCompany);

            _encryptionService.Setup(x => x.DecryptAndDeserialize<Company>(It.IsAny<string>())).Returns(company);
            _encryptionService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(encryptedCompany);

            // Act
            var result = _controller.CreateCompany(encryptedCompany);

            // Assert
            
        }

    }
}
