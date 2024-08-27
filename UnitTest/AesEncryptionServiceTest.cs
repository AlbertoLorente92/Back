using Back.Implementation;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTest
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

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");

            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);

            _aesEncryptionService = new AesEncryptionService(_mockConfiguration.Object);
        }

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
    }
}
