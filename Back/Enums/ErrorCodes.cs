using System.ComponentModel;

namespace Back.Enums
{
    public enum ErrorCodes
    {
        #region General errors
        #region Encryption
        [Description("Encrypted payload is missing.")]
        PayloadIsMissing = 1001,
        [Description("Decrypted payload is invalid.")]
        PayloadDecryptionFailed = 1002,
        [Description("Invalid input arguments.")]
        InvalidDecryptedData = 1003,
        #endregion Encryption

        #region Properties
        [Description("Unmodifiable property")]
        UnmodifiableProperty = 1010,
        [Description("Unique property")]
        UniqueProperty = 1011,
        [Description("Non existent property")]
        NonExistentProperty = 1012,
        [Description("Property castinge error")]
        PropertyCastingError = 1013,
        #endregion Properties

        [Description("Unknown error.")]
        UnknownError = 9999,
        #endregion General errors

        #region Company errors
        #region Create
        [Description("Vat already exists on database")]
        VatAlreadyExists = 2001,
        #endregion Create
        #region Update
        [Description("Company does not exists on database")]
        CompanyDoesNotExist = 2002,
        #endregion Update
        #endregion Company errors


        
        #region User errors
        #region Create
        [Description("Email already exists on database")]
        EmailAlreadyExists = 3001,
        #endregion Create
        #region Update
        [Description("User does not exists on database")]
        UserDoesNotExist = 3002,
        #endregion Update
        #endregion User errors
    }
}
