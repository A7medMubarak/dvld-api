using DVLD.Contracts.Requests.User;
using FluentValidation;

namespace DVLD.Contracts.Validators.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role.");
        }
    }
}
