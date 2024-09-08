using Back.Implementation;

namespace UnitTest.Services
{
    public class PasswordHashServiceTest
    {
        private PasswordHashService _passwordHashService;

        [SetUp]
        public void Setup()
        {
            _passwordHashService = new PasswordHashService();
        }

        [Test]
        public void HashPassword()
        {
            var password = "password";
            var hashedPassword = _passwordHashService.HashPassword(password);

            Assert.That(password, Is.Not.EqualTo(hashedPassword.HashedPassword));
        }

        [Test]
        public void VerifyPassword()
        {
            var password = "password";
            var hashedPassword = _passwordHashService.HashPassword(password);

            var correctPassword = _passwordHashService.VerifyPassword(password, hashedPassword.HashedPassword, hashedPassword.Salt);

            Assert.That(correctPassword, Is.True);

        }
    }
}
