using Back.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Back.Implementation
{
    #pragma warning disable S101 // Types should be named in PascalCase
    public class AESEncryption : IMessageEncryption
    #pragma warning restore S101 // Types should be named in PascalCase
    {
        private readonly ICryptoTransform _decryptor;
        private readonly ICryptoTransform _encryptor;

        public AESEncryption(IConfiguration configuration)
        {
            var aesSecretKey = configuration.GetValue<string>("AesSecretKey") ?? string.Empty;
            var aesIV = configuration.GetValue<string>("AesIV") ?? string.Empty;
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(aesSecretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(aesIV);
            _encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            _decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        }

        public string Decrypt(string encriptMessage)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encriptMessage);
            using var msDecrypt = new MemoryStream(encryptedBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, _decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        public string Encrypt(string plainMessage)
        {
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, _encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainMessage);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
    }
}
