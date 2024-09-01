namespace Back.Interfaces
{
    public interface ITextEncryptionService
    {
        public string Decrypt(string encriptMessage);
        public T? DecryptAndDeserialize<T>(string encriptMessage) where T : class;
        public string Encrypt(string plainMessage);
        public string SerielizeAndEncrypt(object objectToEncrypt);

    }
}
