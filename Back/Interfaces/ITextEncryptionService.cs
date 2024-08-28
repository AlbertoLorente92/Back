namespace Back.Interfaces
{
    public interface ITextEncryptionService
    {
        public string Decrypt(string encriptMessage);
        public string Encrypt(string plainMessage);
        public T? DecryptAndDeserialize<T>(string encriptMessage) where T : class;
    }
}
