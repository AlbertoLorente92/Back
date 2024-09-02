namespace Back.Models
{
    public class UpdateCompanyRequest
    {
        public Guid Guid { get; set; }
        public required Dictionary<string, object> Data { get; set; }
    }
}
