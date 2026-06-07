using DVLD.Contracts.Requests.Driver;
using FluentValidation;

namespace DVLD.Contracts.Validators.Driver
{
    public class CreateDriverRequestValidator : AbstractValidator<CreateDriverRequest>
    {
        public CreateDriverRequestValidator()
        {
            RuleFor(x => x.PersonId)
                .GreaterThan(0).WithMessage("Person is required.");
        }
    }
}
