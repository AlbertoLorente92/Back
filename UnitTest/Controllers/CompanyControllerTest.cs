using Back.Controllers;
using Back.Enums;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Http;
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
        private Mock<ICompanyService> _companyService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<CompanyController>>();
            _encryptionService = new Mock<ITextEncryptionService>();
            _companyService = new Mock<ICompanyService>();

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


            _controller = new CompanyController(
                logger: mockLogger.Object, 
                textEncryption: _encryptionService.Object,
                configuration: _mockConfiguration.Object,
                companyService: _companyService.Object
            );
        }

        #region CreateCompany method
        [Test]
        public void GetCompanyById_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var companyIdEncrypted = "companyIdEncrypted";
            var companyId = 1;

            var company = new CompanyEntity()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R",
                Guid = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(companyId.ToString());
            _companyService.Setup(x => x.GetCompanyById(It.IsAny<int>())).Returns(company);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(company));


            // Act
            var result = _controller.GetCompanyById(companyIdEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<CompanyEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<CompanyEntity>());
            Assert.That(okResultValue, Is.EqualTo(company));
        }

        [Test]
        public void GetCompanyById_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetCompanyById(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetCompanyById_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetCompanyById(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetCompanyById_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var getCompanyById = "getCompanyById";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string)null!);

            // Act
            var result = _controller.GetCompanyById(getCompanyById);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void GetCompanyById_WithErrorWhenParseInputData_ShouldBadRequest()
        {
            // Arrange
            var getCompanyById = "getCompanyById";
            var companyId = "failedInteger";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(companyId);

            // Act
            var result = _controller.GetCompanyById(getCompanyById);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.InvalidDecryptedData));
        }

        [Test]
        public void GetCompanyById_WhenCompanyIdDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var getCompanyById = "getCompanyById";
            var companyId = 1;

            var createCompanyResponse = new CreateCompanyResponse()
            {
                SaveCompanyCode = SaveCompanyCode.VatAlreadyExists,
                Company = null
            };

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(companyId.ToString);
            _companyService.Setup(x => x.GetCompanyById(It.IsAny<int>())).Returns((CompanyEntity)null!);

            // Act
            var result = _controller.GetCompanyById(getCompanyById);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetCompanyById_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.GetCompanyById(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion CreateCompany method

        #region CreateCompany method
        [Test]
        public void CreateCompany_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R"
            };

            var companyCreated = new CompanyEntity()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R",
                Guid = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            var createCompanyResponse = new CreateCompanyResponse()
            {
                SaveCompanyCode = SaveCompanyCode.Ok,
                Company = companyCreated
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateCompanyRequest>(It.IsAny<string>())).Returns(createCompanyRequest);
            _companyService.Setup(x => x.CreateCompany(It.IsAny<CreateCompanyRequest>())).Returns(createCompanyResponse);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(companyCreated));


            // Act
            var result = _controller.CreateCompany(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<CompanyEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<CompanyEntity>());
            Assert.That(okResultValue, Is.EqualTo(companyCreated));
        }

        [Test]
        public void CreateCompany_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.CreateCompany(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void CreateCompany_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.CreateCompany(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void CreateCompany_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateCompanyRequest>(It.IsAny<string>())).Returns((CreateCompanyRequest)null!);

            // Act
            var result = _controller.CreateCompany(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void CreateCompany_WhenVatAlreadyExists_ShouldReturnBadRequest()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R"
            };

            var createCompanyResponse = new CreateCompanyResponse()
            {
                SaveCompanyCode = SaveCompanyCode.VatAlreadyExists,
                Company = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateCompanyRequest>(It.IsAny<string>())).Returns(createCompanyRequest);
            _companyService.Setup(x => x.CreateCompany(It.IsAny<CreateCompanyRequest>())).Returns(createCompanyResponse);

            // Act
            var result = _controller.CreateCompany(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.VatAlreadyExists));
        }

        [Test]
        public void CreateCompany_WhenUnknowErrorInCompanyService_ShouldReturnBadRequest()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R"
            };

            var createCompanyResponse = new CreateCompanyResponse()
            {
                SaveCompanyCode = SaveCompanyCode.UnknownError,
                Company = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateCompanyRequest>(It.IsAny<string>())).Returns(createCompanyRequest);
            _companyService.Setup(x => x.CreateCompany(It.IsAny<CreateCompanyRequest>())).Returns(createCompanyResponse);

            // Act
            var result = _controller.CreateCompany(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UnknownError));
        }

        [Test]
        public void CreateCompany_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var createCompanyRequestEncrypted = "createCompanyRequestEncrypted";
            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateCompanyRequest>(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.CreateCompany(createCompanyRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion CreateCompany method
    }
}
