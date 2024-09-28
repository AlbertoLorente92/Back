using Back.Controllers;
using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Resources;

namespace UnitTest.Controllers
{
    [TestFixture]
    //[Ignore("This is for manual testing only")]
    public class JwtControllerManualTest
    {
        private JwtController _controller;
        private ITextEncryptionService _realEncryptionService;
        private IUsers _realUsers;
        private IUserService _userService;
        private IPasswordHashService _passwordHashService;
        private IJwtService _jwtService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<JwtController>>();

            var mockSectionUsersFile = new Mock<IConfigurationSection>();
            mockSectionUsersFile.Setup(x => x.Value).Returns(@"C:\hlocal\TACO\Back\Back\FilesDB\UsersManualTesting.txt");

            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("7xhmi2I24nLtzcOPsKPNKg==");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("TACO_AES_ENCRYPT");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config.GetSection("UsersFile")).Returns(mockSectionUsersFile.Object);
            mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);


            _realEncryptionService = new AesEncryptionService(mockConfiguration.Object);
            _realUsers = new Users(_realEncryptionService, mockConfiguration.Object);

            
            var resourceManager = new ResourceManager("Back.Resources.SharedResource", typeof(SharedResource).Assembly);
            var sharedLocalizer = new Common.ResourceManagerStringLocalizer(resourceManager);

            _passwordHashService = new PasswordHashService();
            _userService = new UserService(new Mock<ILogger<UserService>>().Object, _realUsers, _passwordHashService, sharedLocalizer);

            _jwtService = new JwtService(_userService);

            _controller = new JwtController(mockLogger.Object, _realEncryptionService, mockConfiguration.Object, _jwtService);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Accept-Language"] = "en";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public void GetToken_TryMe()
        {
            // Arrange
            var getTokenRequest = new GetTokenRequest() { Email = "alberto.lorente92@yopmail.com", Password = "123" };
            var encrypted = _realEncryptionService.SerielizeAndEncrypt(getTokenRequest);

            // Act
            var result = _controller.GetToken(encrypted);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var token = _realEncryptionService.Decrypt((string)okObjectResult?.Value!);
            Console.WriteLine(token);            
        }
    }
}
