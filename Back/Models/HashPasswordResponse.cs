namespace Back.Models
{
    public class HashPasswordResponse
    {
        public required string HashedPassword { get; set; }
        public required string Salt { get; set; }
    }
}
