using Back.Enums;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Services
{
    [TestFixture]
    public class CompanyServiceTest
    {
        private ICompanyService _companyServiceTest;
        private Mock<ICompanies> _companies;

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

            _companyServiceTest = new CompanyService(
                logger: mockLogger.Object,
                companies: _companies.Object
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
                ComercialName = "Company C",
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
                ComercialName = "Company C",
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
    }
}
