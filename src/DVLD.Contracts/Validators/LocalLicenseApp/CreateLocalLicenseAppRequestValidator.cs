using DVLD.Contracts.Requests.LocalLicenseApp;
using FluentValidation;

namespace DVLD.Contracts.Validators.LocalLicenseApp
{
    public class CreateLocalLicenseAppRequestValidator : AbstractValidator<CreateLocalLicenseAppRequest>
    {
        public CreateLocalLicenseAppRequestValidator()
        {
            RuleFor(x => x.ApplicantPersonId)
                .GreaterThan(0).WithMessage("Applicant person is required.");

            RuleFor(x => x.LicenseClassId)
                .GreaterThan(0).WithMessage("License class is required.");
        }
    }
}
