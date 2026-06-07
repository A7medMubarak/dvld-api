using DVLD.Contracts.Requests.Test;
using FluentValidation;

namespace DVLD.Contracts.Validators.Test
{
    public class TestWriteRequestValidator : AbstractValidator<TestWriteRequest>
    {
        public TestWriteRequestValidator()
        {
            RuleFor(x => x.TestAppointmentId)
                .GreaterThan(0).WithMessage("Test appointment is required.");
        }
    }
}
