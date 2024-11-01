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
    public class CompanyController(
        ILogger<CompanyController> logger
            , ITextEncryptionService textEncryption
            , ICompanyService companyService) : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger = logger;
        private readonly ITextEncryptionService _textEncryption = textEncryption;
        private readonly ICompanyService _companyService = companyService;

        #region Get methods
        [HttpGet("GetCompanyById", Name = "GetCompanyById")]
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

                return ApiResponseHelper.NotFound();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "GetCompanyById");
                return ApiResponseHelper.Error();
            }
        }

        [HttpGet("GetCompanyByGuid", Name = "GetCompanyByGuid")]
        public ActionResult GetCompanyByGuid(string companyGuidEncrypted)
        {
            if (string.IsNullOrEmpty(companyGuidEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var companyGuid = _textEncryption.Decrypt(companyGuidEncrypted);

                if (companyGuid == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if (!Guid.TryParse(companyGuid, out Guid guid))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var company = _companyService.GetCompanyByGuid(guid);
                if (company != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(company));
                }

                return ApiResponseHelper.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanyByGuid");
                return ApiResponseHelper.Error();
            }
        }

        [HttpGet("GetCompanyByVat", Name = "GetCompanyByVat")]
        public ActionResult GetCompanyByVat(string companyVatEncrypted)
        {
            if (string.IsNullOrEmpty(companyVatEncrypted))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var companyVat = _textEncryption.Decrypt(companyVatEncrypted);

                if (companyVat == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                if (string.IsNullOrEmpty(companyVat))
                {
                    return ApiResponseHelper.BadRequestInvalidData();
                }

                var company = _companyService.GetCompanyByVat(companyVat);
                if (company != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(company));
                }

                return ApiResponseHelper.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanyByVat");
                return ApiResponseHelper.Error();
            }
        }

        [HttpGet("GetCompanies", Name = "GetCompanies")]
        public ActionResult GetCompanies()
        {
            try
            {
                var companies = _companyService.GetCompanies();
                if (companies != null)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(companies));
                }

                return ApiResponseHelper.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanies");
                return ApiResponseHelper.Error();
            }
        }
        #endregion Get methods

        #region Post methods
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

                ErrorCodes response = CompanyMapper.Map(result.SaveCompanyCode);
                return BadRequest(new ErrorResponse(response, response.GetDescription()));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateCompany");
                return ApiResponseHelper.Error();
            }
        }

        [HttpPost("UpdateCompany", Name = "UpdateCompany")]
        public ActionResult UpdateCompany(string encryptedCompanyUpdateRequest)
        {
            if (string.IsNullOrEmpty(encryptedCompanyUpdateRequest))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var companyUpdateRequest = _textEncryption.DecryptAndDeserialize<UpdateCompanyRequest>(encryptedCompanyUpdateRequest);

                if (companyUpdateRequest == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                var result = _companyService.UpdateCompany(companyUpdateRequest);
                if (result.UpdateCompanyCode == UpdateCompanyCode.Ok)
                {
                    return Ok(_textEncryption.SerielizeAndEncrypt(result.Company!));
                }

                ErrorCodes response = CompanyMapper.Map(result.UpdateCompanyCode);
                return BadRequest(new ErrorResponse(response, result.ErrorMessage != string.Empty ? result.ErrorMessage : response.GetDescription()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCompany");
                return ApiResponseHelper.Error();
            }
        }
        #endregion Post methods
    }
}
