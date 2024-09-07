using Back.Interfaces;
using Back.Models;
using System.Text;

namespace Back.Implementation
{
    public class Companies : ICompanies
    {
        private List<CompanyEntity>? _companies = null;

        private const string COMPANIES_FILE = "CompaniesFile";
        private readonly string _filePath;
        private readonly ITextEncryptionService _textEncryptionService;

        public Companies(
            ITextEncryptionService textEncryptionService
            , IConfiguration configuration)
        {
            _textEncryptionService = textEncryptionService;
            _filePath = configuration.GetValue<string>(COMPANIES_FILE) ?? string.Empty;
        }
        List<CompanyEntity> ICompanies.Companies 
        { 
            get 
            {
                _companies ??= GetCompanies();
                return _companies;
            } 
        }

        public bool CreateCompany(CompanyEntity companyEntity)
        {
            try
            {
                _companies ??= GetCompanies();
                _companies.Add(companyEntity);
                var encryptedCompany = _textEncryptionService.SerielizeAndEncrypt(companyEntity);
                using var writer = new StreamWriter(_filePath, append: true, encoding: new UTF8Encoding(false));
                writer.WriteLine(encryptedCompany);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCompany(CompanyEntity companyEntity)
        {
            try
            {
                _companies ??= GetCompanies();
                var company = _companies.FirstOrDefault(c => c.Equals(companyEntity));
                if (company == null)
                {
                    return false;
                }
                var encryptedCompany = _textEncryptionService.SerielizeAndEncrypt(companyEntity);
                var lines = new List<string>(File.ReadAllLines(_filePath, encoding: new UTF8Encoding(false)));
                lines[company.Id - 1] = encryptedCompany;
                File.WriteAllLines(_filePath, lines, encoding: new UTF8Encoding(false));

                company = companyEntity;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<CompanyEntity> GetCompanies()
        {
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
    }
}
