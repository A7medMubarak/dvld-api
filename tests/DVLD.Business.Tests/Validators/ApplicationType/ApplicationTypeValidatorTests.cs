using DVLD.Contracts.Requests.ApplicationType;
using DVLD.Contracts.Validators.ApplicationType;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.ApplicationType
{
    public class ApplicationTypeValidatorTests
    {
        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var request = new CreateApplicationTypeRequest
            {
                ApplicationTypeTitle = "New License",
                ApplicationTypeFees = 100
            };

            var result = new CreateApplicationTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_EmptyTitle_Fails()
        {
            var request = new CreateApplicationTypeRequest
            {
                ApplicationTypeTitle = string.Empty,
                ApplicationTypeFees = 100
            };

            var result = new CreateApplicationTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(WriteApplicationTypeRequest.ApplicationTypeTitle));
        }

        [Fact]
        public void Update_ZeroFees_Fails()
        {
            var request = new UpdateApplicationTypeRequest
            {
                ApplicationTypeTitle = "New License",
                ApplicationTypeFees = 0
            };

            var result = new UpdateApplicationTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
