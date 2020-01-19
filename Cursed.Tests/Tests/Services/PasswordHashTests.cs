using Xunit;
using Cursed.Models.Services;

namespace Cursed.Tests.Tests.Services
{
    public class PasswordHashTests
    {
        private readonly PasswordHash passwordHash;
        public PasswordHashTests()
        {
            passwordHash = new PasswordHash();
        }

        [Fact]
        public void HashGenrated_HashDifferentFromPassword_ReturnsTrue()
        {
            // arrange
            var password = "iC@nB3R3@llP@$$w0rd";
            // act
            var hash = passwordHash.GenerateHash(password);
            // assert
            Assert.NotEqual(hash, password);
        }

        [Fact]
        public void HashGenratedAndCompared_HashMatchingPassword_ReturnsTrue()
        {
            // arrange
            var password = "iC@nB3R3@llP@$$w0rd";
            // act
            var hash = passwordHash.GenerateHash(password);
            var compareResult = passwordHash.IsPasswordMathcingHash(password, hash);
            // assert
            Assert.True(compareResult);
        }

        [Fact]
        public void HashGenratedAndCompared_HashNotMathcingNotPassword_ReturnsFalse()
        {
            // arrange
            var password = "iC@nB3R3@llP@$$w0rd";
            var notAPassword = "qwerty12345";
            // act
            var hash = passwordHash.GenerateHash(password);
            var compareResult = passwordHash.IsPasswordMathcingHash(notAPassword, hash);
            // assert
            Assert.False(compareResult);
        }
    }
}
