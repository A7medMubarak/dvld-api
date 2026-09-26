using DVLD.Contracts.Requests.License;
using DVLD.Contracts.Validators.License;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.License
{
    public class LicenseValidatorTests
    {
        private static CreateLicenseRequest ValidCreate() => new()
        {
            ApplicationId = 1,
            DriverId = 1,
            LicenseClassId = 1,
            IssueReason = 1
        };

        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var result = new CreateLicenseRequestValidator().Validate(ValidCreate());

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_ZeroApplicationId_Fails()
        {
            var request = ValidCreate();
            request.ApplicationId = 0;

            var result = new CreateLicenseRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LicenseWriteRequest.ApplicationId));
        }

        [Fact]
        public void Create_UnknownLicenseClass_Fails()
        {
            var request = ValidCreate();
            request.LicenseClassId = 99;

            var result = new CreateLicenseRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LicenseWriteRequest.LicenseClassId));
        }

        [Fact]
        public void Update_ZeroDriverId_Fails()
        {
            var request = new UpdateLicenseRequest
            {
                ApplicationId = 1,
                DriverId = 0,
                LicenseClassId = 1,
                IssueReason = 1
            };

            var result = new UpdateLicenseRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LicenseWriteRequest.DriverId));
        }
    }
}
