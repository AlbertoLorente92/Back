namespace Back.Models
{
    public class GetTokenRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
