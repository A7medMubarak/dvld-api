using DVLD.Contracts.Requests.Auth;
using FluentValidation;

namespace DVLD.Contracts.Validators.Auth
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
        }
    }
}
