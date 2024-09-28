using Back.Common;
using Back.Interfaces;
using Back.Models;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JwtController : ControllerBase
    {
        private readonly ILogger<JwtController> _logger;
        private readonly ITextEncryptionService _textEncryption;
        private readonly IJwtService _jwtService;
        public JwtController(
            ILogger<JwtController> logger
            , ITextEncryptionService textEncryption
            , IConfiguration configuration
            , IJwtService jwtService)
        {
            _logger = logger;
            _textEncryption = textEncryption;
            _jwtService = jwtService;
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
                var credentials = _textEncryption.DecryptAndDeserialize<GetTokenRequest>(encryptedCredentials);

                if (credentials == null)
                {
                    return ApiResponseHelper.BadRequestDecryptionFailed();
                }

                var token = _jwtService.GetToken(credentials);
                if (token != null)
                {
                    return Ok(_textEncryption.Encrypt(token));
                }

                return BadRequest();
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
