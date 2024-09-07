using Back.Enums;

namespace Back.Models
{
    public class CreateCompanyResponse
    {
        public CompanyEntity? Company { get; set; }
        public required SaveCompanyCode SaveCompanyCode { get; set; }
    }
}
