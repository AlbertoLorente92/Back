using Back.Enums;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Back.Implementation
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IUsers _users;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IStringLocalizer<SharedResource> _sharedResources;

        public UserService(
            ILogger<UserService> logger
            , IUsers users
            , IPasswordHashService passwordHashService
            , IStringLocalizer<SharedResource> sharedResources
            )
        {
            _logger = logger;
            _users = users;
            _passwordHashService = passwordHashService;
            _sharedResources = sharedResources;
        }

        public UserEntity? GetUserById(int id)
        {
            return _users.Users.FirstOrDefault(user => user.Id == id);
        }

        public UserEntity? GetUserByEmail(string email)
        {
            return _users.Users.FirstOrDefault(user => user.Email == email);
        }

        public UserEntity? GetUserByGuid(Guid guid)
        {
            return _users.Users.FirstOrDefault(user => user.Guid == guid);
        }
        public List<UserEntity> GetUsers()
        {
            return _users.Users;
        }
        public CreateUserResponse CreateUser(CreateUserRequest request)
        {
            return SaveNewUser(request);
        }
        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            var user = GetUserByGuid(request.Guid);
            if (user == null)
            {
                return new UpdateUserResponse() 
                { 
                    UpdateUserCode = UpdateUserCode.UserDoesNotExist,
                    ErrorMessage = _sharedResources["UserDoesNotExist"]
                };
            }

            var sanitizeResponse = SanitizeRequestUpdateUser(user, request);
            if (sanitizeResponse.UpdateUserCode != UpdateUserCode.Ok)
            {
                return new UpdateUserResponse()
                {
                    UpdateUserCode = sanitizeResponse.UpdateUserCode,
                    ErrorMessage = sanitizeResponse.ErrorMessage
                };
            }

            if (_users.UpdateUser(user))
            {
                return new UpdateUserResponse() { User = user, UpdateUserCode = UpdateUserCode.Ok };
            }

            return new UpdateUserResponse() { UpdateUserCode = UpdateUserCode.UnknownError };
        }

        

        #region Private methods
        private CreateUserResponse SaveNewUser(CreateUserRequest userRequest)
        {
            if (UserAlreadyExists(userRequest.Email))
            {
                return new CreateUserResponse() { SaveUserCode = SaveUserCode.EmailAlreadyExists };
            }

            var user = CreateNewUser(userRequest);
            if (_users.CreateUser(user))
            {
                return new CreateUserResponse() { SaveUserCode = SaveUserCode.Ok, User = user };
            }

            return new CreateUserResponse() { SaveUserCode = SaveUserCode.UnknownError };         
        }

        private bool UserAlreadyExists(string email)
        {
            return GetUserByEmail(email) != null;
        }

        private UserEntity CreateNewUser(CreateUserRequest userRequest)
        {
            int id = 1;
            if (_users.Users.Any())
            {
                id = _users.Users.OrderByDescending(user => user.Id).First().Id + 1;
            }
            var hashedPassword = _passwordHashService.HashPassword(userRequest.Password);
            return CompanyMapper.MapToEntity(userRequest, Guid.NewGuid(), id, hashedPassword.HashedPassword, hashedPassword.Salt);
        }

        private SanitizeRequestUpdateUserResponse SanitizeRequestUpdateUser(UserEntity user, UpdateUserRequest request)
        {
            string? propertyName = null;
            PropertyInfo? propertyInfo = null;

            try
            {
                foreach (var update in request.Data)
                {
                    propertyName = update.Key;
                    object? newValue = update.Value;

                    propertyInfo = typeof(CompanyEntity).GetProperty(propertyName);
                    if (propertyInfo == null || !propertyInfo.CanWrite)
                    {
                        return new SanitizeRequestUpdateUserResponse()
                        {
                            User = null,
                            ErrorMessage = string.Format(_sharedResources["NonExistentProperty"], _sharedResources[propertyName]),
                            UpdateUserCode = UpdateUserCode.NonExistentProperty
                        };
                    }

                    var columnProperty = propertyInfo?.GetCustomAttributes<ColumnControlLabelAttribute>().FirstOrDefault()!.Label;
                    if (columnProperty == ColumnProperty.Unmodifiable)
                    {
                        return new SanitizeRequestUpdateUserResponse() 
                        {
                            User = null,
                            ErrorMessage = string.Format(_sharedResources["UnmodifiableProperty"], _sharedResources[propertyName]),
                            UpdateUserCode = UpdateUserCode.UnmodifiableProperty
                        };
                    }

                    if (columnProperty == ColumnProperty.Unique && !PropertyValueIsUnique(user.Guid, propertyInfo!, newValue))
                    {
                        return new SanitizeRequestUpdateUserResponse()
                        {
                            User = null,
                            ErrorMessage = string.Format(_sharedResources["UniqueProperty"], _sharedResources[propertyName]),
                            UpdateUserCode = UpdateUserCode.UniqueProperty
                        };
                    }

                    var convertedValue = Convert.ChangeType(newValue, propertyInfo!.PropertyType);
                    propertyInfo.SetValue(user, convertedValue);
                }

                return new SanitizeRequestUpdateUserResponse()
                {
                    User = user,
                    UpdateUserCode = UpdateUserCode.Ok
                };
            }
            catch
            {
                return new SanitizeRequestUpdateUserResponse()
                {
                    User = null,
                    ErrorMessage = string.Format(_sharedResources["PropertyCastingError"], _sharedResources[propertyName!], propertyInfo?.PropertyType.Name!),
                    UpdateUserCode = UpdateUserCode.PropertyCastingError
                };
            }
        }

        private bool PropertyValueIsUnique(Guid userGuid, PropertyInfo propertyInfo, object newValue)
        {
            if (_users.Users.Where(c => c.Guid != userGuid).Any(company =>
            {
                var propertyValue = propertyInfo.GetValue(company);
                return propertyValue != null && propertyValue.Equals(newValue);
            }))
            {
                return false;
            }
            return true;
        }

        private sealed class SanitizeRequestUpdateUserResponse
        {
            public UserEntity? User { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public required UpdateUserCode UpdateUserCode { get; set; }
        }
        #endregion Private methods
    }
}
