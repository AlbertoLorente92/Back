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

        public CompanyController(
            ILogger<CompanyController> logger
            , ITextEncryptionService messageEncryption)
        {
            _logger = logger;
            _messageEncryption = messageEncryption;
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



                return Ok(_messageEncryption.Encrypt(JsonConvert.SerializeObject(new DoesTheWeatherMatchResponse() { IsSuccess = true })));
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

    }
}
