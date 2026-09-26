using DVLD.Contracts.Requests.Application;
using DVLD.Contracts.Validators.Application;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.Application
{
    public class UpdatedStatusValidatorTests
    {
        [Fact]
        public void KnownStatus_Passes()
        {
            var result = new UpdatedStatusRequestValidator().Validate(new UpdatedStatusRequest { NewStatus = 2 });

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void UnknownStatus_Fails()
        {
            var result = new UpdatedStatusRequestValidator().Validate(new UpdatedStatusRequest { NewStatus = 99 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdatedStatusRequest.NewStatus));
        }
    }
}
