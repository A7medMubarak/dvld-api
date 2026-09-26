using DVLD.Contracts.Common;
using DVLD.Contracts.Validators.Common;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.Common
{
    public class PaginationParamsValidatorTests
    {
        [Fact]
        public void Defaults_Pass()
        {
            var result = new PaginationParamsValidator().Validate(new PaginationParams());

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ZeroPageNumber_Fails()
        {
            var result = new PaginationParamsValidator().Validate(new PaginationParams { PageNumber = 0 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationParams.PageNumber));
        }

        [Fact]
        public void ZeroPageSize_Fails()
        {
            var result = new PaginationParamsValidator().Validate(new PaginationParams { PageSize = 0 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(PaginationParams.PageSize));
        }

        [Fact]
        public void OversizedPageSize_IsClampedBySetter_ThenPasses()
        {
            // The init accessor clamps to MaxPageSize before validation runs.
            var result = new PaginationParamsValidator().Validate(new PaginationParams { PageSize = 100 });

            result.IsValid.Should().BeTrue();
        }
    }
}
