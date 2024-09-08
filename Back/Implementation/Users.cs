using Back.Interfaces;
using Back.Models;
using System.Text;

namespace Back.Implementation
{
    public class Users : IUsers
    {
        private List<UserEntity>? _users = null;

        private const string USERS_FILE = "UsersFile";
        private readonly string _filePath;
        private readonly ITextEncryptionService _textEncryptionService;

        public Users(
            ITextEncryptionService textEncryptionService
            , IConfiguration configuration)
        {
            _textEncryptionService = textEncryptionService;
            _filePath = configuration.GetValue<string>(USERS_FILE) ?? string.Empty;
        }
        List<UserEntity> IUsers.Users 
        { 
            get 
            {
                _users ??= GetUsers();
                return _users;
            } 
        }

        public bool CreateUser(UserEntity userEntity)
        {
            try
            {
                _users ??= GetUsers();
                _users.Add(userEntity);
                var encryptedCompany = _textEncryptionService.SerielizeAndEncrypt(userEntity);
                using var writer = new StreamWriter(_filePath, append: true, encoding: new UTF8Encoding(false));
                writer.WriteLine(encryptedCompany);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(UserEntity userEntity)
        {
            try
            {
                _users ??= GetUsers();
                var user = _users.FirstOrDefault(c => c.Equals(userEntity));
                if (user == null)
                {
                    return false;
                }
                var encryptedUser = _textEncryptionService.SerielizeAndEncrypt(userEntity);
                var lines = new List<string>(File.ReadAllLines(_filePath, encoding: new UTF8Encoding(false)));
                lines[user.Id - 1] = encryptedUser;
                File.WriteAllLines(_filePath, lines, encoding: new UTF8Encoding(false));

                user = userEntity;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<UserEntity> GetUsers()
        {
            _users = new List<UserEntity>();
            try
            {
                using var reader = new StreamReader(_filePath, encoding: new UTF8Encoding(false));
                string line;
                while ((line = reader.ReadLine()!) != null)
                {
                    var company = _textEncryptionService.DecryptAndDeserialize<UserEntity>(line!);
                    if (company != null)
                    {
                        _users.Add(company);
                    }
                }
                return _users;
            }
            catch
            {
                return _users;
            }
        }
    }
}
