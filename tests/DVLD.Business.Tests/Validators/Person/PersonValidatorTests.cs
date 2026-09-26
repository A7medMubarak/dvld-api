using DVLD.Contracts.Requests.Person;
using DVLD.Contracts.Validators.Person;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.Person
{
    public class PersonValidatorTests
    {
        private static CreatePersonRequest ValidCreate() => new()
        {
            FirstName = "Ahmed",
            SecondName = "Mohamed",
            LastName = "Ali",
            NationalNo = "29801011234567",
            NationalityCountryId = 1
        };

        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var result = new CreatePersonRequestValidator().Validate(ValidCreate());

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_EmptyFirstName_FailsOnFirstName()
        {
            var request = ValidCreate();
            request.FirstName = string.Empty;

            var result = new CreatePersonRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(PersonWriteRequest.FirstName));
        }

        [Fact]
        public void Create_ZeroCountryId_Fails()
        {
            var request = ValidCreate();
            request.NationalityCountryId = 0;

            var result = new CreatePersonRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Update_EmptyLastName_FailsOnLastName()
        {
            var request = new UpdatePersonRequest
            {
                FirstName = "Ahmed",
                SecondName = "Mohamed",
                LastName = string.Empty,
                NationalNo = "29801011234567",
                NationalityCountryId = 1
            };

            var result = new UpdatePersonRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(PersonWriteRequest.LastName));
        }
    }
}
