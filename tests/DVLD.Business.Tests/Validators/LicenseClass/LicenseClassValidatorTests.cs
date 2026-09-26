using DVLD.Contracts.Requests.LicenseClass;
using DVLD.Contracts.Validators.LicenseClass;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.LicenseClass
{
    public class LicenseClassValidatorTests
    {
        private static CreateLicenseClassRequest ValidCreate() => new()
        {
            ClassName = "Class 3",
            ClassDescription = "Ordinary driving license",
            ClassFees = 100,
            DefaultValidityLength = 10,
            MinimumAllowedAge = 18
        };

        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var result = new CreateLicenseClassRequestValidator().Validate(ValidCreate());

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_EmptyClassName_Fails()
        {
            var request = ValidCreate();
            request.ClassName = string.Empty;

            var result = new CreateLicenseClassRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LicenseClassWriteRequest.ClassName));
        }

        [Fact]
        public void Create_UnderageMinimum_Fails()
        {
            var request = ValidCreate();
            request.MinimumAllowedAge = 16;

            var result = new CreateLicenseClassRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LicenseClassWriteRequest.MinimumAllowedAge));
        }

        [Fact]
        public void Update_EmptyDescription_Fails()
        {
            var request = new UpdateLicenseClassRequest
            {
                ClassName = "Class 3",
                ClassDescription = string.Empty,
                ClassFees = 100,
                DefaultValidityLength = 10,
                MinimumAllowedAge = 18
            };

            var result = new UpdateLicenseClassRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
