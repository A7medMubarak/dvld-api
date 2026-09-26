using DVLD.Contracts.Requests.Test;
using DVLD.Contracts.Validators.Test;
using FluentAssertions;

namespace DVLD.Business.Tests.Validators.Test
{
    public class TestValidatorTests
    {
        [Fact]
        public void Create_ValidRequest_Passes()
        {
            var result = new CreateTestRequestValidator().Validate(new CreateTestRequest { TestAppointmentId = 1 });

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Create_ZeroAppointmentId_Fails()
        {
            var result = new CreateTestRequestValidator().Validate(new CreateTestRequest { TestAppointmentId = 0 });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(TestWriteRequest.TestAppointmentId));
        }

        [Fact]
        public void Update_ZeroAppointmentId_Fails()
        {
            var result = new UpdateTestRequestValidator().Validate(new UpdateTestRequest { TestAppointmentId = 0 });

            result.IsValid.Should().BeFalse();
        }
    }
}
