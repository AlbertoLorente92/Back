namespace Back.Models
{
    public class UpdateUserRequest
    {
        public required Guid Guid { get; set; }
        public required Dictionary<string, object> Data { get; set; }
    }
}
