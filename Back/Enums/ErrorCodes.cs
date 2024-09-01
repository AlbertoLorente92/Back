using System.ComponentModel;

namespace Back.Enums
{
    public enum ErrorCodes
    {
        #region General errors
        [Description("Encrypted payload is missing.")]
        PayloadIsMissing = 1001,
        [Description("Decrypted payload is invalid.")]
        PayloadDecryptionFailed = 1002,
        [Description("Invalid input arguments.")]
        InvalidDecryptedData = 1003,

        [Description("Unknown error.")]
        UnknownError = 9999,
        #endregion General errors

        #region Company errors
        [Description("Vat already exists on database")]
        VatAlreadyExists = 2001,
        #endregion General errors
    }
}
