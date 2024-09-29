using Back.Models;

namespace Back.Interfaces
{
    public interface ICompanyService
    {
        public CompanyEntity? GetCompanyById(int id);
        public CompanyEntity? GetCompanyByVat(string vat);
        public CompanyEntity? GetCompanyByGuid(Guid guid);
        public List<CompanyEntity> GetCompanies();
        public CreateCompanyResponse CreateCompany(CreateCompanyRequest request);
        public UpdateCompanyResponse UpdateCompany(UpdateCompanyRequest request);
    }
}
