using Back.Common;
using Back.Enums;
using Back.Extensions;
using Back.Interfaces;
using Back.Mappers;
using Back.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ITextEncryptionService _textEncryption;
        private readonly ICompanyService _companyService;
        public CompanyController(
            ILogger<CompanyController> logger
            , ITextEncryptionService textEncryption
            , IConfiguration configuration
            , ICompanyService companyService)
        {
            _logger = logger;
            _textEncryption = textEncryption;
            _companyService = companyService;
        }

        [HttpPost("GetCompanyById", Name = "GetCompanyById")]
        public ActionResult GetCompanyById(string companyIdEncrypted)
        {
            if (string.IsNullOrEmpty(companyIdEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var companyId = _textEncryption.Decrypt(companyIdEncrypted);

                if (companyId == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if(!int.TryParse(companyId, out int id))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var company = _companyService.GetCompanyById(id);
                if (company != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(company));
                }

                return NotFound("Company not found");
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "GetCompanyById");
                return Problem("An error occurred");
            }
        }

        [HttpPost("CreateCompany", Name = "CreateCompany")]
        public ActionResult CreateCompany(string encryptedCompanyCreationRequest)
        {
            if (string.IsNullOrEmpty(encryptedCompanyCreationRequest))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var companyCreationRequest = _textEncryption.DecryptAndDeserialize<CreateCompanyRequest>(encryptedCompanyCreationRequest);

                if (companyCreationRequest == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                var result = _companyService.CreateCompany(companyCreationRequest);
                if (result.SaveCompanyCode == SaveCompanyCode.Ok)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(result.Company!));
                }

                ErrorCodes response = CompanyMapper.Map((SaveCompanyCode)result.SaveCompanyCode!);
                return BadRequest(new ErrorResponse(response, response.GetDescription()));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateCompany");
                return Problem("An error occurred");
            }
        }
    }
}
