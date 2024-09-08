using Back.Enums;

namespace Back.Models
{
    public class CreateUserResponse
    {
        public required SaveUserCode SaveUserCode { get; set; }
        public UserEntity? User { get; set; }
    }
}
