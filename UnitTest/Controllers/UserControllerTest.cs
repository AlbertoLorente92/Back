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
    public class UserControllerTest
    {
        private UserController _controller;
        private Mock<ITextEncryptionService> _encryptionService;
        private Mock<IUserService> _userService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<UserController>>();
            _encryptionService = new Mock<ITextEncryptionService>();
            _userService = new Mock<IUserService>();

            var mockSectionCompaniesFile = new Mock<IConfigurationSection>();
            mockSectionCompaniesFile.Setup(x => x.Value).Returns("FilesDB\\Users.txt");

            var mockSectionAesSecretKey = new Mock<IConfigurationSection>();
            mockSectionAesSecretKey.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAAAAAAAAAA");

            var mockSectionAesIV = new Mock<IConfigurationSection>();
            mockSectionAesIV.Setup(x => x.Value).Returns("AAAAAAAAAAAAAAAA");


            var _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config.GetSection("CompaniesFile")).Returns(mockSectionCompaniesFile.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesSecretKey")).Returns(mockSectionAesSecretKey.Object);
            _mockConfiguration.Setup(config => config.GetSection("AesIV")).Returns(mockSectionAesIV.Object);


            _controller = new UserController(
                logger: mockLogger.Object, 
                textEncryption: _encryptionService.Object,
                configuration: _mockConfiguration.Object,
                userService: _userService.Object
            );
        }

        #region GetUserById method
        [Test]
        public void GetUserById_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var userIdEncrypted = "userIdEncrypted";
            var userId = 1;

            var user = new UserEntity()
            {
                Name = "Name",
                LastName = "LastName",
                Email = "email@email.com",
                Guid = Guid.NewGuid(),
                Password = "password",
                Salt = "salt",
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userId.ToString());
            _userService.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(user);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(user));


            // Act
            var result = _controller.GetUserById(userIdEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<UserEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<UserEntity>());
            Assert.That(okResultValue, Is.EqualTo(user));
        }

        [Test]
        public void GetUserById_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserById(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserById_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserById(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserById_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var getUserById = "getUserById";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string)null!);

            // Act
            var result = _controller.GetUserById(getUserById);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void GetUserById_WithErrorWhenParseInputData_ShouldBadRequest()
        {
            // Arrange
            var getUserById = "getUserById";
            var userId = "failedInteger";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userId);

            // Act
            var result = _controller.GetUserById(getUserById);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.InvalidDecryptedData));
        }

        [Test]
        public void GetUserById_WhenCompanyIdDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var getUserById = "getUserById";
            var companyId = 1;

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(companyId.ToString);
            _userService.Setup(x => x.GetUserById(It.IsAny<int>())).Returns((UserEntity)null!);

            // Act
            var result = _controller.GetUserById(getUserById);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetUserById_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var getUserById = "getUserById";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.GetUserById(getUserById);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion GetUserById method

        #region GetUserByEmail method
        [Test]
        public void GetUserByEmail_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var userEmailEncrypted = "userEmailEncrypted";
            var userEmail = "email@email.com";

            var user = new UserEntity()
            {
                Name = "Name",
                LastName = "LastName",
                Email = "email@email.com",
                Guid = Guid.NewGuid(),
                Password = "password",
                Salt = "salt",
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userEmail);
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).Returns(user);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(user));


            // Act
            var result = _controller.GetUserByEmail(userEmailEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<UserEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<UserEntity>());
            Assert.That(okResultValue, Is.EqualTo(user));
        }

        [Test]
        public void GetUserByEmail_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserByEmail(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserByEmail_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserByEmail(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserByEmail_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var userEmailEncrypted = "userEmailEncrypted";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string)null!);

            // Act
            var result = _controller.GetUserByEmail(userEmailEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void GetUserByEmail_WithErrorWhenParseInputData_ShouldBadRequest()
        {
            // Arrange
            var userEmailEncrypted = "userEmailEncrypted";
            var companyVat = string.Empty;

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(companyVat);

            // Act
            var result = _controller.GetUserByEmail(userEmailEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.InvalidDecryptedData));
        }

        [Test]
        public void GetUserByEmail_WhenVatDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var userEmailEncrypted = "userEmailEncrypted";
            var userEmail = "email@email.com";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userEmail);
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).Returns((UserEntity)null!);

            // Act
            var result = _controller.GetUserByEmail(userEmailEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetUserByEmail_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var userEmailEncrypted = "userEmailEncrypted";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.GetUserByEmail(userEmailEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion GetUserByEmail method

        #region GetUserByGuid method
        [Test]
        public void GetUserByGuid_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var userGuidEncrypted = "userGuidEncrypted";
            var userGuid = Guid.NewGuid();

            var user = new UserEntity()
            {
                Name = "Name",
                LastName = "LastName",
                Email = "email@email.com",
                Guid = Guid.NewGuid(),
                Password = "password",
                Salt = "salt",
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userGuid.ToString());
            _userService.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).Returns(user);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(user));


            // Act
            var result = _controller.GetUserByGuid(userGuidEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<UserEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<UserEntity>());
            Assert.That(okResultValue, Is.EqualTo(user));
        }

        [Test]
        public void GetUserByGuid_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserByGuid(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserByGuid_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.GetUserByGuid(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void GetUserByGuid_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var userGuidEncrypted = "userGuidEncrypted";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns((string)null!);

            // Act
            var result = _controller.GetUserByGuid(userGuidEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void GetUserByGuid_WithErrorWhenParseInputData_ShouldBadRequest()
        {
            // Arrange
            var userGuidEncrypted = "userGuidEncrypted";
            var userGuid = "stringThatsNotAGuid";

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userGuid);

            // Act
            var result = _controller.GetUserByGuid(userGuidEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.InvalidDecryptedData));
        }

        [Test]
        public void GetUserByGuid_WhenCompanyGuidDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var userGuidEncrypted = "userGuidEncrypted";
            var userGuid = Guid.NewGuid();

            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns(userGuid.ToString());
            _userService.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).Returns((UserEntity)null!);

            // Act
            var result = _controller.GetUserByGuid(userGuidEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetUserByGuid_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var userGuidEncrypted = "userGuidEncrypted";
            _encryptionService.Setup(x => x.Decrypt(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.GetUserByGuid(userGuidEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion GetUserByGuid method

        #region GetUsers method
        [Test]
        public void GetUsers_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var users = new List<UserEntity>() { 
                new() {
                    Name = "Name",
                    LastName = "LastName",
                    Email = "email@email.com",
                    Guid = Guid.NewGuid(),
                    Password = "password",
                    Salt = "salt",
                    CreationDate = DateTime.UtcNow,
                    Id = 1,
                    Deleted = false,
                }
            };

            _userService.Setup(x => x.GetUsers()).Returns(users);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(users));

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<List<UserEntity>>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<List<UserEntity>>());
            Assert.That(okResultValue.Count, Is.EqualTo(users.Count));
            foreach(var company in okResultValue){
                Assert.That(company, Is.EqualTo(users.FirstOrDefault(c => c.Equals(company))));
            }
        }

        [Test]
        public void GetUsers_WhenCompaniesDoesNotExists_ShouldReturnNotFound()
        {
            // Arrange
            _userService.Setup(x => x.GetUsers()).Returns((List<UserEntity>)null!);

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public void GetUsers_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            _userService.Setup(x => x.GetUsers()).Throws(new Exception("exception"));

            // Act
            var result = _controller.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion GetUsers method

        #region CreateUser method
        [Test]
        public void CreateUser_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var createUserRequestEncrypted = "createUserRequestEncrypted";
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "email@email.com",
                Password = "password"
            };

            var userCreated = new UserEntity()
            {
                Name = "Name",
                LastName = "LastName",
                Email = "email@email.com",
                Guid = Guid.NewGuid(),
                Password = "password",
                Salt = "salt",
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            var createUserResponse = new CreateUserResponse()
            {
                SaveUserCode = SaveUserCode.Ok,
                User = userCreated
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateUserRequest>(It.IsAny<string>())).Returns(createUserRequest);
            _userService.Setup(x => x.CreateUser(It.IsAny<CreateUserRequest>())).Returns(createUserResponse);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(userCreated));


            // Act
            var result = _controller.CreateUser(createUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<UserEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<UserEntity>());
            Assert.That(okResultValue, Is.EqualTo(userCreated));
        }

        [Test]
        public void CreateUser_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.CreateUser(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void CreateUser_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.CreateUser(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }

        [Test]
        public void CreateUser_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var createUserRequestEncrypted = "createUserRequestEncrypted";

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateUserRequest>(It.IsAny<string>())).Returns((CreateUserRequest)null!);

            // Act
            var result = _controller.CreateUser(createUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void CreateUser_WhenEmailAlreadyExists_ShouldReturnBadRequest()
        {
            // Arrange
            var createUserRequestEncrypted = "createUserRequestEncrypted";
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "email@email.com",
                Password = "password"
            };

            var createUserResponse = new CreateUserResponse()
            {
                SaveUserCode = SaveUserCode.EmailAlreadyExists,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateUserRequest>(It.IsAny<string>())).Returns(createUserRequest);
            _userService.Setup(x => x.CreateUser(It.IsAny<CreateUserRequest>())).Returns(createUserResponse);

            // Act
            var result = _controller.CreateUser(createUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.EmailAlreadyExists));
        }

        [Test]
        public void CreateUser_WhenUnknowErrorInCompanyService_ShouldReturnBadRequest()
        {
            // Arrange
            var createUserRequestEncrypted = "createUserRequestEncrypted";
            var createUserRequest = new CreateUserRequest()
            {
                Name = "Alberto",
                LastName = "Lorente",
                Email = "email@email.com",
                Password = "password"
            };

            var createUserResponse = new CreateUserResponse()
            {
                SaveUserCode = SaveUserCode.UnknownError,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateUserRequest>(It.IsAny<string>())).Returns(createUserRequest);
            _userService.Setup(x => x.CreateUser(It.IsAny<CreateUserRequest>())).Returns(createUserResponse);

            // Act
            var result = _controller.CreateUser(createUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UnknownError));
        }

        [Test]
        public void CreateUser_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var createUserRequestEncrypted = "createUserRequestEncrypted";
            _encryptionService.Setup(x => x.DecryptAndDeserialize<CreateUserRequest>(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.CreateUser(createUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }
        #endregion CreateUser method

        #region UpdateCompany method
        [Test]
        public void UpdateUser_WithEverythingCorrect_ShouldReturnOk()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var userUpdated = new UserEntity()
            {
                Name = "Name",
                LastName = "LastName",
                Email = "email@email.com",
                Guid = Guid.NewGuid(),
                Password = "password",
                Salt = "salt",
                CreationDate = DateTime.UtcNow,
                Id = 1,
                Deleted = false,
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.Ok,
                User = userUpdated
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);
            _encryptionService.Setup(x => x.SerielizeAndEncrypt(It.IsAny<object>())).Returns(JsonConvert.SerializeObject(userUpdated));


            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var okResult = result as OkObjectResult;
            var okResultValue = JsonConvert.DeserializeObject<UserEntity>((string?)(okResult?.Value)!);

            Assert.That(okResultValue, Is.InstanceOf<UserEntity>());
            Assert.That(okResultValue, Is.EqualTo(userUpdated));
        }
        
        [Test]
        public void UpdateUser_WhenPayloadIsNull_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.UpdateUser(null!);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }
        
        [Test]
        public void UpdateUser_WhenPayloadIsEmpty_ShouldBadRequest()
        {
            // Arrange


            // Act
            var result = _controller.UpdateUser(string.Empty);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadIsMissing));
        }
        
        [Test]
        public void UpdateUser_WithErrorWhenDecrypt_ShouldBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns((UpdateUserRequest)null!);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PayloadDecryptionFailed));
        }

        [Test]
        public void UpdateUser_WhenUserDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.UserDoesNotExist,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UserDoesNotExist));
        }

        [Test]
        public void UpdateUser_WhenUniqueProperty_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.UniqueProperty,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UniqueProperty));
        }

        [Test]
        public void UpdateUser_WhenUnmodifiableProperty_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.UnmodifiableProperty,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UnmodifiableProperty));
        }

        [Test]
        public void UpdateUser_WhenPropertyCastingError_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.PropertyCastingError,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.PropertyCastingError));
        }

        [Test]
        public void UpdateUser_WhenNonExistentProperty_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.NonExistentProperty,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.NonExistentProperty));
        }
        
        [Test]
        public void UpdateUser_WhenUnknowErrorInCompanyService_ShouldReturnBadRequest()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            var updateUserRequest = new UpdateUserRequest()
            {
                Guid = Guid.NewGuid(),
                Data = new() { { "Name", "Nombre" } }
            };

            var updateUserResponse = new UpdateUserResponse()
            {
                UpdateUserCode = UpdateUserCode.UnknownError,
                User = null
            };

            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Returns(updateUserRequest);
            _userService.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequest>())).Returns(updateUserResponse);

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var badRequestObjectResult = result as BadRequestObjectResult;
            var badRequestValue = badRequestObjectResult?.Value as ErrorResponse;

            Assert.That(badRequestValue?.ErrorCode, Is.EqualTo(ErrorCodes.UnknownError));
        }
        
        [Test]
        public void UpdateUser_WhenExceptionOccours_ShouldReturnProblem()
        {
            // Arrange
            var updateUserRequestEncrypted = "updateUserRequestEncrypted";
            _encryptionService.Setup(x => x.DecryptAndDeserialize<UpdateUserRequest>(It.IsAny<string>())).Throws(new Exception("exception"));

            // Act
            var result = _controller.UpdateUser(updateUserRequestEncrypted);

            // Assert
            Assert.That(result, Is.Not.Null);
            var objectResult = result as ObjectResult;

            Assert.That(objectResult?.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        #endregion UpdateUser method
    }
}
