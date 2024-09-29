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
    public class UserControllerManualTest
    {
        private UserController _controller;
        private ITextEncryptionService _realEncryptionService;
        private IUsers _realUsers;
        private IUserService _userService;
        private IPasswordHashService _passwordHashService;
        private IKeyVaultService _keyVaultService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<UserController>>();

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

            _keyVaultService = new KeyVaultService(mockConfiguration.Object);
            _realEncryptionService = new AesEncryptionService(mockConfiguration.Object, _keyVaultService);
            _realUsers = new Users(_realEncryptionService, mockConfiguration.Object);


            var resourceManager = new ResourceManager("Back.Resources.SharedResource", typeof(SharedResource).Assembly);
            var sharedLocalizer = new Common.ResourceManagerStringLocalizer(resourceManager);

            _passwordHashService = new PasswordHashService();
            _userService = new UserService(new Mock<ILogger<UserService>>().Object, _realUsers, _passwordHashService, sharedLocalizer);

            _controller = new UserController(mockLogger.Object, _realEncryptionService, mockConfiguration.Object, _userService);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Accept-Language"] = "en";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public void GetUsers_TryMe()
        {
            // Arrange


            // Act
            var result = _controller.GetUsers();

            // Assert
            var okObjectResult = result as OkObjectResult;

            var companies = _realEncryptionService.DecryptAndDeserialize<List<UserEntity>>((string)okObjectResult?.Value!);

            foreach (var company in companies!) 
            {
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine(company.ToString());
            }
            
        }

        [Test]
        public void CreateUser_TryMe()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "alberto.lorente92@yopmail.com",
                Password = "123",
            };

            var encryptedUser = _realEncryptionService.SerielizeAndEncrypt(createUserRequest);

            // Act
            var result = _controller.CreateUser(encryptedUser);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var user = _realEncryptionService.DecryptAndDeserialize<UserEntity>((string)okObjectResult?.Value!);


            Console.WriteLine(user);
        }

        [Test]
        public void UpdateUser_TryMe()
        {
            // Arrange
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.Parse("9610f8df-32df-4208-9251-c9b61d1acac0"),
                Data = new Dictionary<string, object> { { "Name", "Alberto Alejandro" } }
            };

            var encryptedCompany = _realEncryptionService.SerielizeAndEncrypt(updateUserRequest);

            // Act
            var result = _controller.UpdateUser(encryptedCompany);

            // Assert
            var okObjectResult = result as OkObjectResult;

            var user = _realEncryptionService.DecryptAndDeserialize<UserEntity>((string)okObjectResult?.Value!);

            Console.WriteLine(user);
        }

    }
}
