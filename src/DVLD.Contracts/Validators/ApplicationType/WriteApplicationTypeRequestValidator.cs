using DVLD.Contracts.Requests.ApplicationType;
using FluentValidation;

namespace DVLD.Contracts.Validators.ApplicationType
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class WriteApplicationTypeRequestValidator<T> : AbstractValidator<T>
        where T : WriteApplicationTypeRequest
    {
        protected WriteApplicationTypeRequestValidator()
        {
            RuleFor(x => x.ApplicationTypeTitle)
                .NotEmpty().WithMessage("Application type title is required.");

            RuleFor(x => x.ApplicationTypeFees)
                .GreaterThan(0).WithMessage("Application type fees must be greater than zero.");
        }
    }

    public class CreateApplicationTypeRequestValidator : WriteApplicationTypeRequestValidator<CreateApplicationTypeRequest> { }

    public class UpdateApplicationTypeRequestValidator : WriteApplicationTypeRequestValidator<UpdateApplicationTypeRequest> { }
}
