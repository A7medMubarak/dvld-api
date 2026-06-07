using DVLD.Contracts.Requests.DetainedLicense;
using FluentValidation;

namespace DVLD.Contracts.Validators.DetainedLicense
{
    public class CreateDetainedLicenseRequestValidator : AbstractValidator<CreateDetainedLicenseRequest>
    {
        public CreateDetainedLicenseRequestValidator()
        {
            RuleFor(x => x.LicenseId)
                .GreaterThan(0).WithMessage("License is required.");

            RuleFor(x => x.FineFees)
                .GreaterThanOrEqualTo(5).WithMessage("Fine fees must be at least 5.");
        }
    }
}
