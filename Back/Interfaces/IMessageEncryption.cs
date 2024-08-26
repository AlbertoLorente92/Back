namespace Back.Interfaces
{
    public interface IMessageEncryption
    {
        public string Decrypt(string encriptMessage);
        public string Encrypt(string plainMessage);
    }
}
