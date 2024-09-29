namespace Back.Models
{
    public class CreateCompanyRequest
    {
        public required string Name { get; set; }
        public required string Vat { get; set; }
    }
}
