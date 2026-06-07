using DVLD.Contracts.Requests.Auth;
using FluentValidation;

namespace DVLD.Contracts.Validators.Auth
{
    public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
    {
        public LogoutRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
