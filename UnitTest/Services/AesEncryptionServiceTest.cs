using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;

namespace UnitTest.Services
{
    [TestFixture]
    public class AesEncryptionServiceTest
    {
        private AesEncryptionService _aesEncryptionService;
        private const string PLAIN_TEXT = "Lorem ipsum odor amet, consectetuer adipiscing elit. " +
                "Cubilia finibus inceptos mi sit gravida, interdum cras. " +
                "Varius iaculis lacus eleifend per pharetra penatibus quam posuere donec. " +
                "Tincidunt sollicitudin euismod metus massa suspendisse feugiat ante. " +
                "Lacinia orci condimentum aptent sed et mollis. " +
                "Torquent suspendisse convallis cubilia ullamcorper; ultrices urna torquent.";

        [SetUp]
        public void SetUp()
        {
            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAAAAAAAAAA");
            //mockSectionAesSecretKey.Setup(x => x.Value).Returns("AesSecretKey");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");
            //mockSectionAesIV.Setup(x => x.Value).Returns("SectionAesIV");

            var kvAddres = new Mock<IConfigurationSection>();
            kvAddres.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");

            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("KV:AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("KV:AesIV")).Returns(mockSectionAesIV.Object);
            _mockConfiguration.Setup(config => config.GetSection("KV_ADDRESS")).Returns(kvAddres.Object);

            var keyVaultService = new Mock<IKeyVaultService>();
            keyVaultService
                .Setup(service => service.GetSecret(It.IsAny<string>()))
                .Returns((string secretName) => secretName);



            _aesEncryptionService = new AesEncryptionService(_mockConfiguration.Object, keyVaultService.Object);
        }

        #region Encrypt method

        [Test]
        public void Encrypt_ShouldReturnEncryptedString()
        {
            // Arrange
            string plainText = PLAIN_TEXT;

            // Act
            string encryptedText = _aesEncryptionService.Encrypt(plainText);

            // Assert
            Assert.That(encryptedText, Is.Not.Null);
            Assert.That(encryptedText, Is.Not.EqualTo(plainText));
        }

        [Test]
        public void Encrypt_WhenPlainTextIsNull_ShouldReturnNull()
        {
            // Arrange
            string plainText = null!;

            // Act
            string encryptedText = _aesEncryptionService.Encrypt(plainText);

            // Assert
            Assert.That(encryptedText, Is.Null);
        }

        [Test]
        public void Encrypt_WhenPlainTextIsEmpty_ShouldReturnNull()
        {
            // Arrange
            string plainText = string.Empty;

            // Act
            string encryptedText = _aesEncryptionService.Encrypt(plainText);

            // Assert
            Assert.That(encryptedText, Is.Null);
        }
        #endregion Encrypt method

        #region Decrypt method
        [Test]
        public void Decrypt_ShouldReturnOriginalString()
        {
            // Arrange
            string plainText = PLAIN_TEXT;

            // Act
            string encryptedText = _aesEncryptionService.Encrypt(plainText);
            string decryptedText = _aesEncryptionService.Decrypt(encryptedText);

            // Assert
            Assert.That(decryptedText, Is.Not.Null);
            Assert.That(decryptedText, Is.EqualTo(plainText));
        }

        [Test]
        public void Decrypt_WhenEncryptedTextIsNull_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = null!;

            // Act
            string decryptedText = _aesEncryptionService.Decrypt(encryptedText);

            // Assert
            Assert.That(decryptedText, Is.Null);
        }

        [Test]
        public void Decrypt_WhenEncryptedTextIsEmpty_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = string.Empty;

            // Act
            string decryptedText = _aesEncryptionService.Decrypt(encryptedText);

            // Assert
            Assert.That(decryptedText, Is.Null);
        }
        #endregion Decrypt method

        #region DecryptAndDeserialize method
        [Test]
        public void DecryptAndDeserialize_ShouldReturnOriginalObject()
        {
            // Arrange
            var company = new CompanyEntity()
            {
                Guid = Guid.NewGuid(),
                Id = 1,
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R",
                Deleted = false,
                CreationDate = DateTime.UtcNow
            };

            // Act
            string encryptedText = _aesEncryptionService.Encrypt(JsonConvert.SerializeObject(company));
            var decryptedCompany = _aesEncryptionService.DecryptAndDeserialize<CompanyEntity>(encryptedText);

            // Assert
            Assert.That(decryptedCompany, Is.Not.Null);
            Assert.That(decryptedCompany, Is.InstanceOf<CompanyEntity>());
        }

        [Test]
        public void DecryptAndDeserialize_WhenEncryptedTextIsNull_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = null!;

            // Act
            var decryptedCompany = _aesEncryptionService.DecryptAndDeserialize<CompanyEntity>(encryptedText);

            // Assert
            Assert.That(decryptedCompany, Is.Null);
        }

        [Test]
        public void DecryptAndDeserialize_WhenEncryptedTextIsEmpty_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = string.Empty;

            // Act
            var decryptedCompany = _aesEncryptionService.DecryptAndDeserialize<CompanyEntity>(encryptedText);

            // Assert
            Assert.That(decryptedCompany, Is.Null);
        }
        #endregion DecryptAndDeserialize method

        #region DecryptAndDeserialize method
        [Test]
        public void SerielizeAndEncrypt_ShouldReturnOriginalObject()
        {
            // Arrange
            var company = new CompanyEntity()
            {
                Guid = Guid.NewGuid(),
                Id = 1,
                Name = "Company",
                ComercialName = "Company",
                Vat = "00000001R",
                Deleted = false,
                CreationDate = DateTime.UtcNow
            };

            // Act
            string encryptedText = _aesEncryptionService.SerielizeAndEncrypt(company);
            var decryptedCompany = _aesEncryptionService.DecryptAndDeserialize<CompanyEntity>(encryptedText);

            // Assert
            Assert.That(decryptedCompany, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(decryptedCompany, Is.InstanceOf<CompanyEntity>());
                Assert.That(company, Is.EqualTo(decryptedCompany));
            });
        }

        [Test]
        public void SerielizeAndEncrypt_WhenObjectIsNull_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = null!;

            // Act
            var decryptedCompany = _aesEncryptionService.SerielizeAndEncrypt(encryptedText);

            // Assert
            Assert.That(decryptedCompany, Is.Null);
        }
        #endregion DecryptAndDeserialize method
    }
}
