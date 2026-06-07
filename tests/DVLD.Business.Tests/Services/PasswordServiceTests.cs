using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;

namespace DVLD.Business.Tests.Services
{
    public class PasswordServiceTests
    {
        private readonly IPasswordService _sut = new PasswordService();

        [Fact]
        public void Hash_ReturnsNonEmptyString()
        {
            var result = _sut.Hash("Test@123");

            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Hash_SamePasswordYieldsDifferentHash_EachCall()
        {
            var hash1 = _sut.Hash("Test@123");
            var hash2 = _sut.Hash("Test@123");

            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var hash = _sut.Hash("Test@123");

            var result = _sut.Verify("Test@123", hash);

            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_IncorrectPassword_ReturnsFalse()
        {
            var hash = _sut.Hash("Test@123");

            var result = _sut.Verify("WrongPassword", hash);

            result.Should().BeFalse();
        }

        [Fact]
        public void Verify_NullPassword_ThrowsArgumentNullException()
        {
            var hash = _sut.Hash("Test@123");

            var act = () => _sut.Verify(null!, hash);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Verify_NullHash_ThrowsArgumentNullException()
        {
            var act = () => _sut.Verify("Test@123", null!);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
