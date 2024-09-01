using Back.Enums;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;
using System.Text;

namespace Back.Implementation
{
    public class CompanyService : ICompanyService
    {
        private const string COMPANIES_FILE = "CompaniesFile";
        private List<CompanyEntity>? _companies = null;
        private readonly string _filePath;
        private readonly ILogger _logger;
        private readonly ITextEncryptionService _textEncryptionService;

        public CompanyService(
            ILogger<CompanyService> logger
            , ITextEncryptionService textEncryptionService
            , IConfiguration configuration)
        {
            _logger = logger;
            _textEncryptionService = textEncryptionService;
            _filePath = configuration.GetValue<string>(COMPANIES_FILE) ?? string.Empty;
        }

        public CompanyEntity? GetCompanyById(int id)
        {
            _companies ??= GetCompanies();

            return _companies.FirstOrDefault(company => company.Id == id);
        }

        public CompanyEntity? GetCompanyByVat(string vat)
        {
            _companies ??= GetCompanies();

            return _companies.FirstOrDefault(company => company.Vat == vat);
        }

        public CompanyEntity? GetCompanyByGuid(Guid guid)
        {
            _companies ??= GetCompanies();

            return _companies.FirstOrDefault(company => company.Guid == guid);
        }
        public List<CompanyEntity> GetCompanies()
        {
            if(_companies != null)
            {
                return _companies;
            }

            _companies = new List<CompanyEntity>();
            try
            {
                using var reader = new StreamReader(_filePath, encoding: new UTF8Encoding(false));
                string line;
                while ((line = reader.ReadLine()!) != null)
                {
                    var company = _textEncryptionService.DecryptAndDeserialize<CompanyEntity>(line!);
                    if (company != null)
                    {
                        _companies.Add(company);
                    }
                }
                return _companies;
            }
            catch
            {
                return _companies;
            }
        }
        public CreateCompanyResponse CreateCompany(CreateCompanyRequest request)
        {
            _companies ??= GetCompanies();
            return SaveCompany(request);
        }

        #region Private methods
        private CreateCompanyResponse SaveCompany(CreateCompanyRequest companyRequest)
        {
            var response = new CreateCompanyResponse();
            try
            {
                if (CompanyAlreadyExists(companyRequest.Vat))
                {
                    response.SaveCompanyCode = SaveCompanyCode.VatAlreadyExists;
                    return response;
                }
                var company = CreateNewCompany(companyRequest);
                var encryptedCompany = _textEncryptionService.SerielizeAndEncrypt(company);
                using var writer = new StreamWriter(_filePath, append: true, encoding: new UTF8Encoding(false));
                writer.WriteLine(encryptedCompany);
                response.SaveCompanyCode = SaveCompanyCode.Ok;
                response.Company = company;
                return response;
            }
            catch
            {
                response.SaveCompanyCode = SaveCompanyCode.UnknownError;
                return response;
            }
        }
        private bool CompanyAlreadyExists(string vat)
        {
            var company = GetCompanyByVat(vat);
            return company != null;
        }

        private CompanyEntity CreateNewCompany(CreateCompanyRequest companyRequest)
        {
            int id = 1;
            if (_companies != null && _companies.Any())
            {
                id = _companies.OrderByDescending(company => company.Id).First().Id + 1;
            }

            return CompanyMapper.MapToEntity(companyRequest, Guid.NewGuid(), id);
        }
        #endregion Private methods
    }
}
