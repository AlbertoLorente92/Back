using Back.Controllers;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Globalization;
using System.Resources;

namespace UnitTest.Controllers
{
    [TestFixture]
    [Ignore("This is for manual testing only")]
    public class CompanyControllerManualTest
    {
        private CompanyController _controller;
        private ITextEncryptionService _realEncryptionService;
        private ICompanies _realCompanies;
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

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config.GetSection("CompaniesFile")).Returns(mockSectionCompaniesFile.Object);
            mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);


            _realEncryptionService = new AesEncryptionService(mockConfiguration.Object);
            _realCompanies = new Companies(_realEncryptionService, mockConfiguration.Object);


            var resourceManager = new ResourceManager("Back.Resources.SharedResource", typeof(SharedResource).Assembly);
            var sharedLocalizer = new Common.ResourceManagerStringLocalizer(resourceManager);


            _companyService = new CompanyService(new Mock<ILogger<CompanyService>>().Object, _realCompanies, sharedLocalizer);

            _controller = new CompanyController(mockLogger.Object, _realEncryptionService, mockConfiguration.Object, _companyService);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Accept-Language"] = "en";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public void GetCompanyById_TryMe()
        {
            // Arrange
            int id = 1;
            var encryptedCompanyId = _realEncryptionService.Encrypt(id.ToString());

            // Act
            var result = _controller.GetCompanyById(encryptedCompanyId);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var company = _realEncryptionService.DecryptAndDeserialize<CompanyEntity>((string)okObjectResult?.Value!);

            var companyString = JsonConvert.SerializeObject(company);

            Console.WriteLine(companyString);
        }

        [Test]
        public void GetCompanyByVat_TryMe()
        {
            // Arrange
            var vat = "00000001R";
            var encryptedVatCompany = _realEncryptionService.Encrypt(vat);

            // Act
            var result = _controller.GetCompanyByVat(encryptedVatCompany);

            // Assert
            var okObjectResult = result as OkObjectResult;
            
            var company = _realEncryptionService.DecryptAndDeserialize<CompanyEntity>((string)okObjectResult?.Value!);

            var companyString = JsonConvert.SerializeObject(company);

            Console.WriteLine(companyString);
        }

        [Test]
        public void GetCompanyByGuid_TryMe()
        {
            // Arrange
            var guid = "6c5986ce-b00b-4ac8-a8d4-0e6860ea3fdb";
            var encryptedCompanyGuid = _realEncryptionService.Encrypt(guid);

            // Act
            var result = _controller.GetCompanyByGuid(encryptedCompanyGuid);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var company = _realEncryptionService.DecryptAndDeserialize<CompanyEntity>((string)okObjectResult?.Value!);

            var companyString = JsonConvert.SerializeObject(company);

            Console.WriteLine(companyString);
        }

        [Test]
        public void GetAllCompanies_TryMe()
        {
            // Arrange


            // Act
            var result = _controller.GetAllCompanies();

            // Assert
            var okObjectResult = result as OkObjectResult;

            var companies = _realEncryptionService.DecryptAndDeserialize<List<CompanyEntity>>((string)okObjectResult?.Value!);

            foreach (var company in companies!) 
            {
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine(company.ToString());
            }
            
        }

        [Test]
        public void CreateCompany_TryMe()
        {
            // Arrange
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Name = "Company B",
                Vat = "00000002R"
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.CreateCompany(encryptedCompany);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var company = _realEncryptionService.DecryptAndDeserialize<CompanyEntity>((string)okObjectResult?.Value!);

            var companyString = JsonConvert.SerializeObject(company);

            Console.WriteLine(companyString);
        }

        [Test]
        public void UpdateCompany_TryMe()
        {
            // Arrange
            var createCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = Guid.Parse("9610f8df-32df-4208-9251-c9b61d1acac0"),
                Data = new Dictionary<string, object> { { "Name", "Alberto3" } }
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.UpdateCompany(encryptedCompany);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var companyEntity = _realEncryptionService.DecryptAndDeserialize<CompanyEntity>((string)okObjectResult?.Value!);

            Console.WriteLine(companyEntity!.ToString());
        }

        [Test]
        public void UpdateCompany_PropDoesNotExists()
        {
            // Arrange
            var createCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = Guid.Parse("9610f8df-32df-4208-9251-c9b61d1acac0"),
                Data = new Dictionary<string, object> { { "NonExistant", Guid.NewGuid() } }
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.UpdateCompany(encryptedCompany);

            // Assert
            var badRequestObjectResult = result as BadRequestObjectResult;
            var response = badRequestObjectResult?.Value as ErrorResponse;

            Console.WriteLine(response!.ErrorCode.ToString());
            Console.WriteLine(response!.Message.ToString());
        }

        [Test]
        public void UpdateCompany_Unmod()
        {
            // Arrange
            var createCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = Guid.Parse("9610f8df-32df-4208-9251-c9b61d1acac0"),
                Data = new Dictionary<string, object> { { "Guid", Guid.NewGuid() } }
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.UpdateCompany(encryptedCompany);

            // Assert
            var badRequestObjectResult = result as BadRequestObjectResult;
            var response = badRequestObjectResult?.Value as ErrorResponse;

            Console.WriteLine(response!.ErrorCode.ToString());
            Console.WriteLine(response!.Message.ToString());
        }

        [Test]
        public void UpdateCompany_Unique()
        {
            // Arrange
            var createCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = Guid.Parse("9610f8df-32df-4208-9251-c9b61d1acac0"),
                Data = new Dictionary<string, object> { { "Vat", "00000001R" } }
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(createCompanyRequest);

            // Act
            var result = _controller.UpdateCompany(encryptedCompany);

            // Assert
            var badRequestObjectResult = result as BadRequestObjectResult;
            var response = badRequestObjectResult?.Value as ErrorResponse;

            Console.WriteLine(response!.ErrorCode.ToString());
            Console.WriteLine(response!.Message.ToString());
        }
    }
}
