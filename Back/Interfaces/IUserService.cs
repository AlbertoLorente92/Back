using Back.Models;

namespace Back.Interfaces
{
    public interface IUserService
    {
        public UserEntity? GetUserById(int id);
        public UserEntity? GetUserByEmail(string email);
        public UserEntity? GetUserByGuid(Guid guid);
        public List<UserEntity> GetUsers();
        public CreateUserResponse CreateUser(CreateUserRequest request);
        public UpdateUserResponse UpdateUser(UpdateUserRequest request);
    }
}
