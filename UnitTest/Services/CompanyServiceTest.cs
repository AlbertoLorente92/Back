using Back.Enums;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Services
{
    [TestFixture]
    public class CompanyServiceTest
    {
        private ICompanyService _companyServiceTest;
        private Mock<ICompanies> _companies;
        private Mock<IStringLocalizer<SharedResource>> _mockLocalizer;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<CompanyService>>();
            _companies = new Mock<ICompanies>();

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

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _mockLocalizer.Setup(l => l["ComercialName"]).Returns(new LocalizedString("ComercialName", "Comercial name"));
            _mockLocalizer.Setup(l => l["CompanyDoesNotExist"]).Returns(new LocalizedString("CompanyDoesNotExist", "Company does not exists"));
            _mockLocalizer.Setup(l => l["CreationDate"]).Returns(new LocalizedString("CreationDate", "Creation date"));
            _mockLocalizer.Setup(l => l["Deleted"]).Returns(new LocalizedString("Deleted", "Deleted"));
            _mockLocalizer.Setup(l => l["Guid"]).Returns(new LocalizedString("Guid", "Guid"));
            _mockLocalizer.Setup(l => l["Id"]).Returns(new LocalizedString("Id", "Id"));
            _mockLocalizer.Setup(l => l["Name"]).Returns(new LocalizedString("Name", "Name"));
            _mockLocalizer.Setup(l => l["NonExistentProperty"]).Returns(new LocalizedString("NonExistentProperty", "The property '{0}' does not exists in the company entity"));
            _mockLocalizer.Setup(l => l["PropertyCastingError"]).Returns(new LocalizedString("PropertyCastingError", "The value given for the property '{0}' is not valid. A '{1}' was expected"));
            _mockLocalizer.Setup(l => l["UniqueProperty"]).Returns(new LocalizedString("UniqueProperty", "The value given for the property '{0}' is already on the database"));
            _mockLocalizer.Setup(l => l["UnmodifiableProperty"]).Returns(new LocalizedString("UnmodifiableProperty", "The property '{0}' can not be modify"));
            _mockLocalizer.Setup(l => l["Vat"]).Returns(new LocalizedString("Vat", "Vat"));


            _companyServiceTest = new CompanyService(
                logger: mockLogger.Object,
                companies: _companies.Object,
                sharedResources: _mockLocalizer.Object
            );
        }

        #region GetCompanyById method
        [Test]
        public void GetCompanyById()
        {
            // Arrange
            var companyId = 1;
            var company = new CompanyEntity() { Guid = Guid.NewGuid(), Id = companyId, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" };
            var mockedCompanies = new List<CompanyEntity>
            {
                company,
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);

            // Act
            var response = _companyServiceTest.GetCompanyById(companyId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(company));
        }
        #endregion GetCompanyById method

        #region GetCompanyByVat method
        [Test]
        public void GetCompanyByVat()
        {
            // Arrange
            var companyVat = "00000001R";
            var company = new CompanyEntity() { Guid = Guid.NewGuid(), Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = companyVat };
            var mockedCompanies = new List<CompanyEntity>
            {
                company,
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);

            // Act
            var response = _companyServiceTest.GetCompanyByVat(companyVat);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(company));
        }
        #endregion GetCompanyByVat method

        #region GetCompanyByGuid method
        [Test]
        public void GetCompanyByGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var company = new CompanyEntity() { Guid = guid, Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" };
            var mockedCompanies = new List<CompanyEntity>
            {
                company,
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);

            // Act
            var response = _companyServiceTest.GetCompanyByGuid(guid);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(company));
        }
        #endregion GetCompanyByGuid method

        #region GetCompanies method
        [Test]
        public void GetCompanies()
        {
            // Arrange

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = Guid.NewGuid(), Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);

            // Act
            var response = _companyServiceTest.GetCompanies();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.All(item1 => mockedCompanies.Contains(item1)), Is.True);
        }
        #endregion GetCompanies method

        #region CreateCompany method
        [Test]
        public void CreateCompany()
        {
            // Arrange
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Vat = "00000003R",
                Name = "Company C",
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = Guid.NewGuid(), Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.CreateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.CreateCompany(createCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SaveCompanyCode, Is.EqualTo(SaveCompanyCode.Ok));
        }

        [Test]
        public void CreateCompany_WhenVatAlreadyExists_ReturnError()
        {
            // Arrange
            var createCompanyRequest = new CreateCompanyRequest()
            {
                Vat = "00000001R",
                Name = "Company C",
            };


            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = Guid.NewGuid(), Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);

            // Act
            var response = _companyServiceTest.CreateCompany(createCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SaveCompanyCode, Is.EqualTo(SaveCompanyCode.VatAlreadyExists));
        }
        #endregion CreateCompany method

        #region UpdateCompany method
        [Test]
        public void UpdateCompany()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var newName = "Company C";
            var updateCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = guid,
                Data = new() { { "Name", newName } }
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = guid, Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.UpdateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.UpdateCompany(updateCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateCompanyCode, Is.EqualTo(UpdateCompanyCode.Ok));
            Assert.That(response.Company!.Name, Is.EqualTo(newName));
        }

        [Test]
        public void UpdateCompany_WhenCompanyDoesNotExist_ReturnError()
        {
            // Arrange
            var updateCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Name" } }
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = Guid.NewGuid(), Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.UpdateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.UpdateCompany(updateCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateCompanyCode, Is.EqualTo(UpdateCompanyCode.CompanyDoesNotExist));
            Assert.That(response.ErrorMessage, Is.EqualTo((string)_mockLocalizer.Object["CompanyDoesNotExist"]));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateCompany_WhenUnmodifiableProperty_ReturnError()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var propName = "Guid";
            var updateCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = guid,
                Data = new() { { propName, Guid.NewGuid() } }
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = guid, Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.UpdateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.UpdateCompany(updateCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateCompanyCode, Is.EqualTo(UpdateCompanyCode.UnmodifiableProperty));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["UnmodifiableProperty"], propName)));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateCompany_WhenUniqueProperty_ReturnError()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var propName = "Vat";
            var updateCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = guid,
                Data = new() { { propName, "00000002R" } }
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = guid, Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.UpdateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.UpdateCompany(updateCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateCompanyCode, Is.EqualTo(UpdateCompanyCode.UniqueProperty));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["UniqueProperty"], propName)));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateCompany_WhenPropertyCastingError_ReturnError()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var propName = "Deleted";
            var updateCompanyRequest = new UpdateCompanyRequest()
            {
                Guid = guid,
                Data = new() { { propName, "NotABoolean" } }
            };

            var mockedCompanies = new List<CompanyEntity>
            {
                new() { Guid = guid, Id = 1, Name = "Company A", ComercialName = "Company A", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000001R" },
                new() { Guid = Guid.NewGuid(), Id = 2, Name = "Company B", ComercialName = "Company B", CreationDate = DateTime.UtcNow, Deleted = false, Vat = "00000002R" },
            };

            _companies.Setup(x => x.Companies).Returns(mockedCompanies);
            _companies.Setup(x => x.UpdateCompany(It.IsAny<CompanyEntity>())).Returns(true);

            // Act
            var response = _companyServiceTest.UpdateCompany(updateCompanyRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateCompanyCode, Is.EqualTo(UpdateCompanyCode.PropertyCastingError));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["PropertyCastingError"], propName, "Boolean")));
            Console.WriteLine(response.ErrorMessage);
        }
        #endregion UpdateCompany method
    }
}
