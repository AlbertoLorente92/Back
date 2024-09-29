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
    public class UserServiceTest
    {
        private IUserService _userServiceTest;
        private Mock<IUsers> _users;
        private Mock<IStringLocalizer<SharedResource>> _mockLocalizer;
        private Mock<IPasswordHashService> _mockPasswordHashService;
        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<UserService>>();
            _users = new Mock<IUsers>();

            var mockSectionUsersFile = new Mock<IConfigurationSection>();
            mockSectionUsersFile.Setup(x => x.Value).Returns("FilesDB\\Users.txt");

            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAAAAAAAAAA");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");

            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("CompaniesFile")).Returns(mockSectionUsersFile.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _mockLocalizer.Setup(l => l["ComercialName"]).Returns(new LocalizedString("ComercialName", "Comercial name"));
            _mockLocalizer.Setup(l => l["UserDoesNotExist"]).Returns(new LocalizedString("UserDoesNotExist", "User does not exists"));
            _mockLocalizer.Setup(l => l["CreationDate"]).Returns(new LocalizedString("CreationDate", "Creation date"));
            _mockLocalizer.Setup(l => l["Deleted"]).Returns(new LocalizedString("Deleted", "Deleted"));
            _mockLocalizer.Setup(l => l["Guid"]).Returns(new LocalizedString("Guid", "Guid"));
            _mockLocalizer.Setup(l => l["Id"]).Returns(new LocalizedString("Id", "Id"));
            _mockLocalizer.Setup(l => l["Name"]).Returns(new LocalizedString("Name", "Name"));
            _mockLocalizer.Setup(l => l["NonExistentProperty"]).Returns(new LocalizedString("NonExistentProperty", "The property '{0}' does not exists in the company entity"));
            _mockLocalizer.Setup(l => l["PropertyCastingError"]).Returns(new LocalizedString("PropertyCastingError", "The value given for the property '{0}' is not valid. A '{1}' was expected"));
            _mockLocalizer.Setup(l => l["UniqueProperty"]).Returns(new LocalizedString("UniqueProperty", "The value given for the property '{0}' is already on the database"));
            _mockLocalizer.Setup(l => l["UnmodifiableProperty"]).Returns(new LocalizedString("UnmodifiableProperty", "The property '{0}' can not be modify"));
            _mockLocalizer.Setup(l => l["Email"]).Returns(new LocalizedString("Email", "Email"));


            _mockPasswordHashService = new Mock<IPasswordHashService>();
            var hashPasswordResponse = new HashPasswordResponse() { HashedPassword = "password", Salt = "salt" };
            _mockPasswordHashService.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(hashPasswordResponse);

            _userServiceTest = new UserService(
                logger: mockLogger.Object,
                users: _users.Object,
                passwordHashService: _mockPasswordHashService.Object,
                sharedResources: _mockLocalizer.Object
            );
        }

        #region GetUserById method
        [Test]
        public void GetUserById()
        {
            // Arrange
            var userId = 1;
            var user = new UserEntity() { 
                Guid = Guid.NewGuid()
                , Id = userId
                , Name = "User A"
                , LastName = "User A"
                , Email = "email@email.com"
                , Password = "password"
                , Salt = "salt"
                , CreationDate = DateTime.UtcNow
                , Deleted = false 
            };
            var mockedUsers = new List<UserEntity>
            {
                user,
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);

            // Act
            var response = _userServiceTest.GetUserById(userId);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(user));
        }
        #endregion GetUserById method

        #region GetUserByEmail method
        [Test]
        public void GetUserByEmail()
        {
            // Arrange
            var userEmail = "email@email.com";
            var user = new UserEntity() { 
                Guid = Guid.NewGuid()
                , Id = 1
                , Name = "User A"
                , LastName = "User A"
                , Email = userEmail
                , Password = "password"
                , Salt = "salt"
                , CreationDate = DateTime.UtcNow
                , Deleted = false 
            };
            var mockedUsers = new List<UserEntity>
            {
                user,
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);

            // Act
            var response = _userServiceTest.GetUserByEmail(userEmail);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(user));
        }
        #endregion GetUserByEmail method

        #region GetUserByGuid method
        [Test]
        public void GetUserByGuid()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var user = new UserEntity() { 
                Guid = userGuid
                , Id = 1
                , Name = "User A"
                , LastName = "User A"
                , Email = "email@email.com"
                , Password = "password"
                , Salt = "salt"
                , CreationDate = DateTime.UtcNow
                , Deleted = false 
            };
            var mockedUsers = new List<UserEntity>
            {
                user,
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);

            // Act
            var response = _userServiceTest.GetUserByGuid(userGuid);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(user));
        }
        #endregion GetUserByGuid method

        #region GetUsers method
        [Test]
        public void GetUsers()
        {
            // Arrange
            var mockedUsers = new List<UserEntity>
            {
                new() { 
                    Guid = Guid.NewGuid()
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false 
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);

            // Act
            var response = _userServiceTest.GetUsers();

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.All(item1 => mockedUsers.Contains(item1)), Is.True);
        }
        #endregion GetUsers method

        #region CreateUser method
        [Test]
        public void CreateUser()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "alberto@email.com",
                Password = "password"
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.CreateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.CreateUser(createUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SaveUserCode, Is.EqualTo(SaveUserCode.Ok));
        }

        [Test]
        public void CreateUser_WhenEmailAlreadyExists_ReturnError()
        {
            // Arrange
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "email@email.com",
                Password = "password"
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);

            // Act
            var response = _userServiceTest.CreateUser(createUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.SaveUserCode, Is.EqualTo(SaveUserCode.EmailAlreadyExists));
        }
        #endregion CreateUser method

        #region UpdateUser method
        [Test]
        public void UpdateUser()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var newName = "Alejandro";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = userGuid,
                Data = new() { { "Name", newName } }
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = userGuid
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.UpdateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.UpdateUser(updateUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Console.WriteLine(response.ErrorMessage);
            Assert.That(response.UpdateUserCode, Is.EqualTo(UpdateUserCode.Ok));
            Assert.That(response.User!.Name, Is.EqualTo(newName));
        }

        [Test]
        public void UpdateUser_WhenUserDoesNotExist_ReturnError()
        {
            // Arrange
            var newName = "Alejandro";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", newName } }
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.UpdateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.UpdateUser(updateUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateUserCode, Is.EqualTo(UpdateUserCode.UserDoesNotExist));
            Assert.That(response.ErrorMessage, Is.EqualTo((string)_mockLocalizer.Object["UserDoesNotExist"]));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateUser_WhenUnmodifiableProperty_ReturnError()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var propName = "Guid";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = userGuid,
                Data = new() { { propName, Guid.NewGuid() } }
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = userGuid
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.UpdateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.UpdateUser(updateUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateUserCode, Is.EqualTo(UpdateUserCode.UnmodifiableProperty));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["UnmodifiableProperty"], propName)));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateUser_WhenUniqueProperty_ReturnError()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var propName = "Email";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = userGuid,
                Data = new() { { propName, "email2@email.com" } }
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = userGuid
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.UpdateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.UpdateUser(updateUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateUserCode, Is.EqualTo(UpdateUserCode.UniqueProperty));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["UniqueProperty"], propName)));
            Console.WriteLine(response.ErrorMessage);
        }

        [Test]
        public void UpdateUser_WhenPropertyCastingError_ReturnError()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var propName = "Deleted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = userGuid,
                Data = new() { { propName, "NotABoolean" } }
            };

            var mockedUsers = new List<UserEntity>
            {
                new() {
                    Guid = userGuid
                    , Id = 1
                    , Name = "User A"
                    , LastName = "User A"
                    , Email = "email@email.com"
                    , Password = "password"
                    , Salt = "salt"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
                new() {
                    Guid = Guid.NewGuid()
                    , Id = 2
                    , Name = "User B"
                    , LastName = "User B"
                    , Email = "email2@email.com"
                    , Password = "password2"
                    , Salt = "salt2"
                    , CreationDate = DateTime.UtcNow
                    , Deleted = false
                },
            };

            _users.Setup(x => x.Users).Returns(mockedUsers);
            _users.Setup(x => x.UpdateUser(It.IsAny<UserEntity>())).Returns(true);

            // Act
            var response = _userServiceTest.UpdateUser(updateUserRequest);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.UpdateUserCode, Is.EqualTo(UpdateUserCode.PropertyCastingError));
            Assert.That(response.ErrorMessage, Is.EqualTo(string.Format(_mockLocalizer.Object["PropertyCastingError"], propName, "Boolean")));
            Console.WriteLine(response.ErrorMessage);
        }
        #endregion UpdateUser method
    }
}
