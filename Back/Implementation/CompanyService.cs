using Back.Enums;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;
using System.Reflection;

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
        public UpdateCompanyResponse UpdateCompany(UpdateCompanyRequest request)
        {
            var company = GetCompanyByGuid(request.Guid);
            if (company == null)
            {
                return new UpdateCompanyResponse() 
                { 
                    UpdateCompanyCode = UpdateCompanyCode.CompanyDoesNotExist,
                    ErrorMessage = "La empresa solicitada no existe"
                };
            }

            var sanitizeResponse = SanitizeRequestUpdateCompany(company, request);
            if (sanitizeResponse.UpdateCompanyCode != UpdateCompanyCode.Ok)
            {
                return new UpdateCompanyResponse()
                {
                    UpdateCompanyCode = sanitizeResponse.UpdateCompanyCode,
                    ErrorMessage = sanitizeResponse.ErrorMessage
                };
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

        private SanitizeRequestUpdateCompanyResponse SanitizeRequestUpdateCompany(CompanyEntity company, UpdateCompanyRequest request)
        {
            string? propertyName = null;
            PropertyInfo? propertyInfo = null;

            try
            {
                foreach (var update in request.Data)
                {
                    propertyName = update.Key;
                    object? newValue = update.Value;

                    propertyInfo = typeof(CompanyEntity).GetProperty(propertyName);
                    if (propertyInfo == null || !propertyInfo.CanWrite)
                    {
                        return new SanitizeRequestUpdateCompanyResponse()
                        {
                            Company = null,
                            ErrorMessage = $"La propiedad '{propertyName}' no existe en la entidad de empresas",
                            UpdateCompanyCode = UpdateCompanyCode.NonExistentProperty
                        };
                    }

                    var columnProperty = propertyInfo?.GetCustomAttributes<ColumnControlLabelAttribute>().FirstOrDefault()!.Label;
                    if (columnProperty == ColumnProperty.Unmodifiable)
                    {
                        return new SanitizeRequestUpdateCompanyResponse() 
                        {
                            Company = null,
                            ErrorMessage = $"La propiedad '{propertyName}' no puede modificarse",
                            UpdateCompanyCode = UpdateCompanyCode.UnmodifiableProperty
                        };
                    }

                    if (columnProperty == ColumnProperty.Unique && !PropertyValueIsUnique(company.Guid, propertyInfo!, newValue))
                    {
                        return new SanitizeRequestUpdateCompanyResponse()
                        {
                            Company = null,
                            ErrorMessage = $"El valor entregado para la propiedad '{propertyName}' ya existe en la base de datos",
                            UpdateCompanyCode = UpdateCompanyCode.UniqueProperty
                        };
                    }

                    var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(company, convertedValue);
                }

                return new SanitizeRequestUpdateCompanyResponse()
                {
                    Company = company,
                    UpdateCompanyCode = UpdateCompanyCode.Ok
                };
            }
            catch
            {
                return new SanitizeRequestUpdateCompanyResponse()
                {
                    Company = null,
                    ErrorMessage = $"El valor entregado para la propiedad '{propertyName}' no es válido. Se espera un {propertyInfo?.PropertyType.Name}",
                    UpdateCompanyCode = UpdateCompanyCode.PropertyCastingError
                };
            }
        }

        private bool PropertyValueIsUnique(Guid companyGuid, PropertyInfo propertyInfo, object newValue)
        {
            if (_companies.Companies.Where(c => c.Guid != companyGuid).Any(company =>
            {
                var propertyValue = propertyInfo.GetValue(company);
                return propertyValue != null && propertyValue.Equals(newValue);
            }))
            {
                return false;
            }
            return true;
        }

        private sealed class SanitizeRequestUpdateCompanyResponse
        {
            public CompanyEntity? Company { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
            public required UpdateCompanyCode UpdateCompanyCode { get; set; }
        }
        #endregion Private methods
    }
}
