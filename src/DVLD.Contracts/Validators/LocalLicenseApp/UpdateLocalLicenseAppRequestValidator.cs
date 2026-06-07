using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.LocalLicenseApp;
using FluentValidation;

namespace DVLD.Contracts.Validators.LocalLicenseApp
{
    public class UpdateLocalLicenseAppRequestValidator : AbstractValidator<UpdateLocalLicenseAppRequest>
    {
        public UpdateLocalLicenseAppRequestValidator()
        {
            RuleFor(x => x.ApplicationStatus)
                .GreaterThan((byte)0).WithMessage("Application status is required.")
                .Must(value => Enum.IsDefined(typeof(enApplicationStatus), value)).WithMessage("Invalid application status.");
        }
    }
}
