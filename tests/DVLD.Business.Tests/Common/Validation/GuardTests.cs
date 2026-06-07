using DVLD.Business.Common.Validation;
using FluentAssertions;

namespace DVLD.Business.Tests.Common.Validation
{
    public class GuardTests
    {
        // --- AgainstNull ---

        [Fact]
        public void AgainstNull_NullValue_ThrowsArgumentNullException()
        {
            var act = () => Guard.AgainstNull(null, "param");

            act.Should().Throw<ArgumentNullException>().WithMessage("*param*");
        }

        [Fact]
        public void AgainstNull_NonNullValue_DoesNotThrow()
        {
            var act = () => Guard.AgainstNull("hello", "param");

            act.Should().NotThrow();
        }

        // --- AgainstNonPositive ---

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void AgainstNonPositive_InvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            var act = () => Guard.AgainstNonPositive(value, "id");

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*id*");
        }

        [Fact]
        public void AgainstNonPositive_ValidValue_DoesNotThrow()
        {
            var act = () => Guard.AgainstNonPositive(1, "id");

            act.Should().NotThrow();
        }

        // --- AgainstInvalidEnum ---

        [Fact]
        public void AgainstInvalidEnum_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            var act = () => Guard.AgainstInvalidEnum<DayOfWeek>(999, "day");

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*day*");
        }

        [Fact]
        public void AgainstInvalidEnum_ValidValue_DoesNotThrow()
        {
            var act = () => Guard.AgainstInvalidEnum<DayOfWeek>(0, "day");

            act.Should().NotThrow();
        }

        // --- AgainstNullOrWhiteSpace ---

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AgainstNullOrWhiteSpace_InvalidValue_ThrowsArgumentException(string? value)
        {
            var act = () => Guard.AgainstNullOrWhiteSpace(value, "name");

            act.Should().Throw<ArgumentException>().WithMessage("*name*");
        }

        [Fact]
        public void AgainstNullOrWhiteSpace_ValidValue_DoesNotThrow()
        {
            var act = () => Guard.AgainstNullOrWhiteSpace("valid", "name");

            act.Should().NotThrow();
        }

        // --- AgainstInvalidFees ---

        [Fact]
        public void AgainstInvalidFees_BelowDefaultMin_ThrowsArgumentOutOfRangeException()
        {
            var act = () => Guard.AgainstInvalidFees(0, "fees");

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*fees*");
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        public void AgainstInvalidFees_AtOrAboveDefaultMin_DoesNotThrow(decimal value)
        {
            var act = () => Guard.AgainstInvalidFees(value, "fees");

            act.Should().NotThrow();
        }

        [Fact]
        public void AgainstInvalidFees_CustomMinValue_ThrowsWhenBelowCustom()
        {
            var act = () => Guard.AgainstInvalidFees(9, "fees", minValue: 10);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*fees*");
        }

        [Fact]
        public void AgainstInvalidFees_AtCustomMinValue_DoesNotThrow()
        {
            var act = () => Guard.AgainstInvalidFees(10, "fees", minValue: 10);

            act.Should().NotThrow();
        }
    }
}
