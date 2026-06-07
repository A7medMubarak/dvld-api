using DVLD.Contracts.Requests.User;
using FluentValidation;

namespace DVLD.Contracts.Validators.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.PersonId)
                .GreaterThan(0).WithMessage("Person is required.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role.");
        }
    }
}
