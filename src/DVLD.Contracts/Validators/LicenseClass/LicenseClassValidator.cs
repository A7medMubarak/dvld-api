using DVLD.Contracts.Requests.LicenseClass;
using FluentValidation;

namespace DVLD.Contracts.Validators.LicenseClass
{
    // Generic base holds the shared rules. Concrete validators below exist so
    // exact-type validator resolution (MVC auto-validation) finds them —
    // a validator registered for the abstract base never fires for derived DTOs.
    public abstract class LicenseClassValidator<T> : AbstractValidator<T>
        where T : LicenseClassWriteRequest
    {
        protected LicenseClassValidator()
        {
            RuleFor(x => x.ClassName)
                .NotEmpty().WithMessage("Class name is required.");

            RuleFor(x => x.ClassDescription)
                .NotEmpty().WithMessage("Class description is required.");

            RuleFor(x => x.ClassFees)
                .GreaterThanOrEqualTo(5).WithMessage("Fees must be at least 5.");

            RuleFor(x => x.DefaultValidityLength)
                .GreaterThan((byte)0).WithMessage("Default validity length must be greater than zero.");

            RuleFor(x => x.MinimumAllowedAge)
                .GreaterThanOrEqualTo((byte)18).WithMessage("Minimum allowed age must be at least 18.");
        }
    }

    public class CreateLicenseClassRequestValidator : LicenseClassValidator<CreateLicenseClassRequest> { }

    public class UpdateLicenseClassRequestValidator : LicenseClassValidator<UpdateLicenseClassRequest> { }
}
