using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.License;
using FluentValidation;

namespace DVLD.Contracts.Validators.License
{
    public class LicenseWriteRequestValidator : AbstractValidator<LicenseWriteRequest>
    {
        public LicenseWriteRequestValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0).WithMessage("Application is required.");

            RuleFor(x => x.DriverId)
                .GreaterThan(0).WithMessage("Driver is required.");

            RuleFor(x => x.LicenseClassId)
                .Must(value => Enum.IsDefined(typeof(enLicenseClasses), value)).WithMessage("Invalid license class.");

            RuleFor(x => x.IssueReason)
                .Must(value => Enum.IsDefined(typeof(enIssueReason), value)).WithMessage("Invalid issue reason.");
        }
    }
}
