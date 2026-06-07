using DVLD.Contracts.Requests.ApplicationType;
using FluentValidation;

namespace DVLD.Contracts.Validators.ApplicationType
{
    public class WriteApplicationTypeRequestValidator : AbstractValidator<WriteApplicationTypeRequest>
    {
        public WriteApplicationTypeRequestValidator()
        {
            RuleFor(x => x.ApplicationTypeTitle)
                .NotEmpty().WithMessage("Application type title is required.");

            RuleFor(x => x.ApplicationTypeFees)
                .GreaterThan(0).WithMessage("Application type fees must be greater than zero.");
        }
    }
}
