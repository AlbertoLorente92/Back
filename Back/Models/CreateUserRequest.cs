namespace Back.Models
{
    public class CreateUserRequest
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
