using Back.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;

namespace Back.Implementation
{
    #pragma warning disable S101 // Types should be named in PascalCase
    public class AESEncryption : IMessageEncryption
    #pragma warning restore S101 // Types should be named in PascalCase
    {
        private readonly string _aesSecretKey;
        private readonly string _aesIV;

        public AESEncryption(IConfiguration configuration)
        {
            _aesSecretKey = configuration.GetValue<string>("AesSecretKey") ?? string.Empty;
            _aesIV = configuration.GetValue<string>("AesIV") ?? string.Empty;
        }

        public string Decrypt(string encriptMessage)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encriptMessage);

            string jsonString = Decrypt(encryptedBytes);

            return jsonString;
        }

        public string Encrypt(string plainMessage)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_aesSecretKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_aesIV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainMessage);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        private string Decrypt(byte[] cipherText)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(_aesSecretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(_aesIV);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
