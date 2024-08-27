namespace Back.Interfaces
{
    public interface ITextEncryptionService
    {
        public string Decrypt(string encriptMessage);
        public string Encrypt(string plainMessage);
    }
}
