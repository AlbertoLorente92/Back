using Back.Models;

namespace Back.Interfaces
{
    public interface ICompanies
    {
        public List<CompanyEntity> Companies { get; }
        public bool CreateCompany(CompanyEntity companyEntity);
        public bool UpdateCompany(CompanyEntity companyEntity);
    }
}
