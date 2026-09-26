using DVLD.Contracts.Requests.TestType;
using FluentValidation;

namespace DVLD.Contracts.Validators.TestType
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class TestTypeWriteRequestValidator<T> : AbstractValidator<T>
        where T : TestTypeWriteRequest
    {
        protected TestTypeWriteRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Fees)
                .GreaterThanOrEqualTo(5).WithMessage("Fees must be at least 5.");
        }
    }

    public class CreateTestTypeRequestValidator : TestTypeWriteRequestValidator<CreateTestTypeRequest> { }

    public class UpdateTestTypeRequestValidator : TestTypeWriteRequestValidator<UpdateTestTypeRequest> { }
}
