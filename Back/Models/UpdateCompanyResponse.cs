using Back.Enums;

namespace Back.Models
{
    public class UpdateCompanyResponse
    {
        public UpdateCompanyCode UpdateCompanyCode { get; set; }
        public CompanyEntity? Company { get; set; }
    }
}
