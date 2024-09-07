using Back.Interfaces;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Back.Implementation
{
    public class AesEncryptionService : ITextEncryptionService
    {
        private const string AES_SECRET_KEY = "AesSecretKey";
        private const string AES_IV_SECRET_KEY = "AesIV";

        private readonly ICryptoTransform _decryptor;
        private readonly ICryptoTransform _encryptor;

        public AesEncryptionService(IConfiguration configuration)
        {
            var aesSecretKey = configuration.GetValue<string>(AES_SECRET_KEY) ?? string.Empty;
            var aesIV = configuration.GetValue<string>(AES_IV_SECRET_KEY) ?? string.Empty;
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(aesSecretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(aesIV);
            _encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            _decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        }

        public string Decrypt(string encriptMessage)
        {
            if (string.IsNullOrEmpty(encriptMessage))
            {
                return null!;
            }
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encriptMessage);
                using var msDecrypt = new MemoryStream(encryptedBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, _decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                return srDecrypt.ReadToEnd();
            }
            catch
            {
                return null!;
            }
        }
        public T? DecryptAndDeserialize<T>(string encriptMessage) where T : class
        {
            if (string.IsNullOrEmpty(encriptMessage))
            {
                return null!;
            }
            try
            {
                var json = Decrypt(encriptMessage);
                return JsonConvert.DeserializeObject<T>(json)!;
            }
            catch
            {
                return null;
            }
        }
        public string Encrypt(string plainMessage)
        {
            if (string.IsNullOrEmpty(plainMessage))
            {
                return null!;
            }
            try
            {
                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, _encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainMessage);
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch
            {
                return null!;
            }
        }

        public string SerielizeAndEncrypt(object objectToEncrypt)
        {
            if (objectToEncrypt == null)
            {
                return null!;
            }
            try
            {
                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, _encryptor, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(JsonConvert.SerializeObject(objectToEncrypt));
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch
            {
                return null!;
            }
        }
    }
}
