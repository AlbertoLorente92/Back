using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ITextEncryptionService _messageEncryption;
        private readonly IConfiguration _configuration;

        public CompanyController(
            ILogger<CompanyController> logger
            , ITextEncryptionService messageEncryption
            , IConfiguration configuration)
        {
            _logger = logger;
            _messageEncryption = messageEncryption;
            _configuration = configuration;
        }

        [HttpPost("CreateCompany", Name = "CreateCompany")]
        public ActionResult CreateCompany(string encryptedCompany)
        {
            if (string.IsNullOrEmpty(encryptedCompany))
            {
                return BadRequest("Encrypted payload is missing.");
            }

            try
            {
                var company = _messageEncryption.DecryptAndDeserialize<Company>(encryptedCompany);

                if (company == null)
                {
                    return BadRequest("Decrypted payload is invalid.");
                }

                if (SaveCompany(encryptedCompany))
                {
                    return Ok();
                }

                return Problem(detail: "An unexpected error occurred.", statusCode: 500);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        private bool SaveCompany(string encryptedCompany)
        {
            try
            {
                var companiesFile = _configuration.GetValue<string>("CompaniesFile") ?? string.Empty;
                var path = Path.Combine(AppContext.BaseDirectory, companiesFile);
                using var writer = new StreamWriter(path, append: true);
                writer.WriteLine(encryptedCompany);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
