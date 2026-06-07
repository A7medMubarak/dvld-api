using DVLD.Contracts.Requests.InternationalLicense;
using FluentValidation;

namespace DVLD.Contracts.Validators.InternationalLicense
{
    public class CreateInternationalLicenseRequestValidator : AbstractValidator<CreateInternationalLicenseRequest>
    {
        public CreateInternationalLicenseRequestValidator()
        {
            RuleFor(x => x.IssuedUsingLocalLicenseId)
                .GreaterThan(0).WithMessage("Local license is required.");

            RuleFor(x => x.DriverId)
                .GreaterThan(0).WithMessage("Driver is required.");
        }
    }
}
