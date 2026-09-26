using DVLD.Contracts.Requests.TestType;
using DVLD.Contracts.Validators.TestType;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.TestType
{
    public class TestTypeValidatorTests
    {
        private static CreateTestTypeRequest ValidCreate() => new()
        {
            Title = "Vision Test",
            Description = "Eyesight examination",
            Fees = 10
        };

        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var result = new CreateTestTypeRequestValidator().Validate(ValidCreate());

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_EmptyTitle_FailsOnTitle()
        {
            var request = ValidCreate();
            request.Title = string.Empty;

            var result = new CreateTestTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(TestTypeWriteRequest.Title));
        }

        [Fact]
        public void Create_FeesBelowMinimum_Fails()
        {
            var request = ValidCreate();
            request.Fees = 1;

            var result = new CreateTestTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(TestTypeWriteRequest.Fees));
        }

        [Fact]
        public void Update_EmptyDescription_Fails()
        {
            var request = new UpdateTestTypeRequest
            {
                Title = "Vision Test",
                Description = string.Empty,
                Fees = 10
            };

            var result = new UpdateTestTypeRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
        }
    }
}
