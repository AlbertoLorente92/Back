using Back.Enums;

namespace Back.Models
{
    public class ErrorResponse
    {
        public ErrorCodes ErrorCode { get; set; }
        public string Message { get; set; }

        public ErrorResponse(ErrorCodes errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
