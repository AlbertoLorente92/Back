namespace Back.Models
{
    public class Company
    {
        public required Guid Guid { get; set; }
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string ComercialName { get; set; }
        public required string Vat { get; set; }
    }
}
