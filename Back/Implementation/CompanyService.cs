using Back.Enums;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;

namespace Back.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly ILogger _logger;
        private readonly ICompanies _companies;

        public CompanyService(
            ILogger<CompanyService> logger
            , ICompanies companies
            )
        {
            _logger = logger;
            _companies = companies;
        }

        public CompanyEntity? GetCompanyById(int id)
        {
            return _companies.Companies.FirstOrDefault(company => company.Id == id);
        }

        public CompanyEntity? GetCompanyByVat(string vat)
        {
            return _companies.Companies.FirstOrDefault(company => company.Vat == vat);
        }

        public CompanyEntity? GetCompanyByGuid(Guid guid)
        {
            return _companies.Companies.FirstOrDefault(company => company.Guid == guid);
        }
        public List<CompanyEntity> GetCompanies()
        {
            return _companies.Companies;
        }
        public CreateCompanyResponse CreateCompany(CreateCompanyRequest request)
        {
            return SaveNewCompany(request);
        }
        public bool UpdateCompany(CompanyEntity companyEntity)
        {
            var company = GetCompanyByGuid(companyEntity.Guid);
            if (company == null)
            {
                return false;
            }

            return true;
        }

        public UpdateCompanyResponse UpdateCompany(UpdateCompanyRequest request)
        {
            var company = GetCompanyByGuid(request.Guid);
            if (company == null)
            {
                return new UpdateCompanyResponse() { UpdateCompanyCode = UpdateCompanyCode.CompanyDoesNotExist};
            }

            if (request.Data.ContainsKey("Guid") || request.Data.ContainsKey("Id"))
            {
                return new UpdateCompanyResponse() { UpdateCompanyCode = UpdateCompanyCode.UnmodifiableProperty };
            }

            if (request.Data.TryGetValue("Vat", out object? value) && CompanyAlreadyExists((string)value))
            {
                return new UpdateCompanyResponse() { UpdateCompanyCode = UpdateCompanyCode.VatAlreadyExists };
            }

            foreach (var update in request.Data)
            {
                var propertyName = update.Key;
                var newValue = update.Value;

                var propertyInfo = typeof(CompanyEntity).GetProperty(propertyName);

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(company, convertedValue);
                }
                else
                {
                    return new UpdateCompanyResponse() { UpdateCompanyCode = UpdateCompanyCode.UnknownError };
                }
            }

            if (_companies.UpdateCompany(company))
            {
                return new UpdateCompanyResponse() { Company = company, UpdateCompanyCode = UpdateCompanyCode.Ok };
            }

            return new UpdateCompanyResponse() { UpdateCompanyCode = UpdateCompanyCode.UnknownError };
        }

        #region Private methods
        private CreateCompanyResponse SaveNewCompany(CreateCompanyRequest companyRequest)
        {
            if (CompanyAlreadyExists(companyRequest.Vat))
            {
                return new CreateCompanyResponse() { SaveCompanyCode = SaveCompanyCode.VatAlreadyExists };
            }

            var company = CreateNewCompany(companyRequest);
            if (_companies.CreateCompany(company))
            {
                return new CreateCompanyResponse() { SaveCompanyCode = SaveCompanyCode.Ok, Company = company };
            }

            return new CreateCompanyResponse() { SaveCompanyCode = SaveCompanyCode.UnknownError };         
        }

        private bool CompanyAlreadyExists(string vat)
        {
            return GetCompanyByVat(vat) != null;
        }

        private CompanyEntity CreateNewCompany(CreateCompanyRequest companyRequest)
        {
            int id = 1;
            if (_companies.Companies.Any())
            {
                id = _companies.Companies.OrderByDescending(company => company.Id).First().Id + 1;
            }

            return CompanyMapper.MapToEntity(companyRequest, Guid.NewGuid(), id);
        }
        #endregion Private methods
    }
}
