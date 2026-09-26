using DVLD.Contracts.Requests.Person;
using FluentValidation;

namespace DVLD.Contracts.Validators.Person
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class PersonWriteRequestValidator<T> : AbstractValidator<T>
        where T : PersonWriteRequest
    {
        protected PersonWriteRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.SecondName)
                .NotEmpty().WithMessage("Second name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.NationalNo)
                .NotEmpty().WithMessage("National number is required.");

            RuleFor(x => x.NationalityCountryId)
                .GreaterThan(0).WithMessage("Nationality country is required.");
        }
    }

    public class CreatePersonRequestValidator : PersonWriteRequestValidator<CreatePersonRequest> { }

    public class UpdatePersonRequestValidator : PersonWriteRequestValidator<UpdatePersonRequest> { }
}
