using Back.Enums;

namespace Back.Models
{
    public class UpdateUserResponse
    {
        public required UpdateUserCode UpdateUserCode { get; set; }
        public UserEntity? User { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
