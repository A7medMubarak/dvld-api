using DVLD.Contracts.Requests.Test;
using FluentValidation;

namespace DVLD.Contracts.Validators.Test
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class TestWriteRequestValidator<T> : AbstractValidator<T>
        where T : TestWriteRequest
    {
        protected TestWriteRequestValidator()
        {
            RuleFor(x => x.TestAppointmentId)
                .GreaterThan(0).WithMessage("Test appointment is required.");
        }
    }

    public class CreateTestRequestValidator : TestWriteRequestValidator<CreateTestRequest> { }

    public class UpdateTestRequestValidator : TestWriteRequestValidator<UpdateTestRequest> { }
}
