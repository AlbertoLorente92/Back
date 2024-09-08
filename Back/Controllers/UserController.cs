using Back.Common;
using Back.Enums;
using Back.Extensions;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ITextEncryptionService _textEncryption;
        private readonly IUserService _userService;
        public UserController(
            ILogger<UserController> logger
            , ITextEncryptionService textEncryption
            , IConfiguration configuration
            , IUserService userService)
        {
            _logger = logger;
            _textEncryption = textEncryption;
            _userService = userService;
        }

        #region Get methods
        [HttpPost("GetUserById", Name = "GetUserById")]
        public ActionResult GetUserById(string userIdEncrypted)
        {
            if (string.IsNullOrEmpty(userIdEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var userId = _textEncryption.Decrypt(userIdEncrypted);

                if (userId == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if(!int.TryParse(userId, out int id))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var user = _userService.GetUserById(id);
                if (user != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(user));
                }

                return NotFound("User not found");
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "GetUserById");
                return Problem("An error occurred");
            }
        }

        [HttpPost("GetUserByGuid", Name = "GetUserByGuid")]
        public ActionResult GetUserByGuid(string userGuidEncrypted)
        {
            if (string.IsNullOrEmpty(userGuidEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var userGuid = _textEncryption.Decrypt(userGuidEncrypted);

                if (userGuid == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if (!Guid.TryParse(userGuid, out Guid guid))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var user = _userService.GetUserByGuid(guid);
                if (user != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(user));
                }

                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByGuid");
                return Problem("An error occurred");
            }
        }

        [HttpPost("GetUserByEmail", Name = "GetUserByEmail")]
        public ActionResult GetUserByEmail(string userEmailEncrypted)
        {
            if (string.IsNullOrEmpty(userEmailEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var userEmail = _textEncryption.Decrypt(userEmailEncrypted);

                if (userEmail == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if (string.IsNullOrEmpty(userEmail))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var user = _userService.GetUserByEmail(userEmail);
                if (user != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(user));
                }

                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByEmail");
                return Problem("An error occurred");
            }
        }

        [HttpPost("GetUsers", Name = "GetUsers")]
        public ActionResult GetUsers()
        {
            try
            {
                var users = _userService.GetUsers();
                if (users != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(users));
                }

                return NotFound("Users not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUsers");
                return Problem("An error occurred");
            }
        }
        #endregion Get methods

        #region Post methods
        [HttpPost("CreateUser", Name = "CreateUser")]
        public ActionResult CreateUser(string encryptedUserCreationRequest)
        {
            if (string.IsNullOrEmpty(encryptedUserCreationRequest))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var userCreationRequest = _textEncryption.DecryptAndDeserialize<CreateUserRequest>(encryptedUserCreationRequest);

                if (userCreationRequest == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                var result = _userService.CreateUser(userCreationRequest);
                if (result.SaveUserCode == SaveUserCode.Ok)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(result.User!));
                }

                ErrorCodes response = CompanyMapper.Map(result.SaveUserCode);
                return BadRequest(new ErrorResponse(response, response.GetDescription()));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateUser");
                return Problem("An error occurred");
            }
        }

        [HttpPost("UpdateUser", Name = "UpdateUser")]
        public ActionResult UpdateUser(string encryptedUserUpdateRequest)
        {
            if (string.IsNullOrEmpty(encryptedUserUpdateRequest))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var userUpdateRequest = _textEncryption.DecryptAndDeserialize<UpdateUserRequest>(encryptedUserUpdateRequest);

                if (userUpdateRequest == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                var result = _userService.UpdateUser(userUpdateRequest);
                if (result.UpdateUserCode == UpdateUserCode.Ok)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(result.User!));
                }

                ErrorCodes response = CompanyMapper.Map(result.UpdateUserCode);
                return BadRequest(new ErrorResponse(response, result.ErrorMessage != string.Empty ? result.ErrorMessage : response.GetDescription()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser");
                return Problem("An error occurred");
            }
        }
        #endregion Post methods
    }
}
