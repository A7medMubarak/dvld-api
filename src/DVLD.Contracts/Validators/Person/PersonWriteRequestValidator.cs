using DVLD.Contracts.Requests.Person;
using FluentValidation;

namespace DVLD.Contracts.Validators.Person
{
    public class PersonWriteRequestValidator : AbstractValidator<PersonWriteRequest>
    {
        public PersonWriteRequestValidator()
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
}
