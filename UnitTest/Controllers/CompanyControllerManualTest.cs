using Back.Controllers;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Controllers
{
    [TestFixture]
    [Ignore("This is for manual testing only")]
    public class CompanyControllerManualTest
    {
        private CompanyController _controller;
        private ITextEncryptionService _realEncryptionService;
        private ICompanyService _companyService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<CompanyController>>();

            var mockSectionCompaniesFile = new Mock<IConfigurationSection>();
            mockSectionCompaniesFile.Setup(x => x.Value).Returns(@"C:\hlocal\TACO\Back\Back\FilesDB\CompaniesManualTesting.txt");

            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("7xhmi2I24nLtzcOPsKPNKg==");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("TACO_AES_ENCRYPT");


            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("CompaniesFile")).Returns(mockSectionCompaniesFile.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);

            _realEncryptionService = new AesEncryptionService(_mockConfiguration.Object);
            _companyService = new CompanyService(new Mock<ILogger<CompanyService>>().Object, _realEncryptionService, _mockConfiguration.Object);
            _controller = new CompanyController(mockLogger.Object, _realEncryptionService, _mockConfiguration.Object, _companyService);
        }

        [Test]
        public void CreateCompany_TryMe()
        {
            // Arrange
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R"
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.CreateCompany(encryptedCompany);

            // Assert
            
        }

        [Test]
        public void GetCompanyById_TryMe()
        {
            // Arrange
            int id = 1;
            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(id);

            // Act
            var result = _controller.GetCompanyById(encryptedCompany);

            // Assert

        }

    }
}
