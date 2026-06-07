using DVLD.Contracts.Requests.TestType;
using FluentValidation;

namespace DVLD.Contracts.Validators.TestType
{
    public class TestTypeWriteRequestValidator : AbstractValidator<TestTypeWriteRequest>
    {
        public TestTypeWriteRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Fees)
                .GreaterThanOrEqualTo(5).WithMessage("Fees must be at least 5.");
        }
    }
}
