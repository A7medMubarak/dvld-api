using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.License;
using FluentValidation;

namespace DVLD.Contracts.Validators.License
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class LicenseWriteRequestValidator<T> : AbstractValidator<T>
        where T : LicenseWriteRequest
    {
        protected LicenseWriteRequestValidator()
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

    public class CreateLicenseRequestValidator : LicenseWriteRequestValidator<CreateLicenseRequest> { }

    public class UpdateLicenseRequestValidator : LicenseWriteRequestValidator<UpdateLicenseRequest> { }
}
