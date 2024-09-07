using Back.Models;

namespace Back.Interfaces
{
    public interface IUsers
    {
        public List<UserEntity> Users { get; }
        public bool CreateUser(UserEntity userEntity);
        public bool UpdateUser(UserEntity userEntity);
    }
}
