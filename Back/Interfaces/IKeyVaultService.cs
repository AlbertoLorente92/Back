namespace Back.Interfaces
{
    public interface IKeyVaultService
    {
        public string? GetSecret(string secretName);
        public Task<string?> GetSecretAsync(string secretName);
    }
}
