using Back.Common;
using Back.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JwtTokenController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ITextEncryptionService _textEncryption;
        private readonly IJwtTokenService _jwtTokenService;
        public JwtTokenController(
            ILogger<CompanyController> logger
            , ITextEncryptionService textEncryption
            , IConfiguration configuration
            , IJwtTokenService jwtTokenService)
        {
            _logger = logger;
            _textEncryption = textEncryption;
            _jwtTokenService = jwtTokenService;
        }

        #region Get methods
        [HttpPost("GetToken", Name = "GetToken")]
        public ActionResult GetToken(string encryptedCredentials)
        {
            if (string.IsNullOrEmpty(encryptedCredentials))
            {
                return ApiResponseHelper.BadRequestMissingPayload();
            }

            try
            {
                var credentials = _textEncryption.Decrypt(encryptedCredentials);

                if (credentials == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }



                return NotFound();
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "GetToken");
                return Problem("An error occurred");
            }
        }
        #endregion Get methods

       
    }
}
