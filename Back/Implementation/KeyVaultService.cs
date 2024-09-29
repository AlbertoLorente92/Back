using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Back.Interfaces;

namespace Back.Implementation
{
    public class KeyVaultService : IKeyVaultService
    {
        private const string KV_ADDRESS = "KV_ADDRESS";
        private readonly string _keyVaultUrl;
        public KeyVaultService(IConfiguration configuration)
        {
            _keyVaultUrl = configuration.GetValue<string>(KV_ADDRESS) ?? string.Empty; 
        }
        public string? GetSecret(string secretName)
        {
            if (string.IsNullOrEmpty(secretName))
            {
                return null;
            }
            try
            {
                var client = new SecretClient(new Uri(_keyVaultUrl), new DefaultAzureCredential());
                var secret = (client.GetSecretAsync(secretName)).GetAwaiter().GetResult();
                return secret.Value.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GetSecretAsync(string secretName)
        {
            if (string.IsNullOrEmpty(secretName))
            {
                return null;
            }
            try
            {
                var client = new SecretClient(new Uri(_keyVaultUrl), new DefaultAzureCredential());
                var secret = await client.GetSecretAsync(secretName);
                return secret.Value.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
