using DVLD.Contracts.Requests.LicenseClass;
using FluentValidation;

namespace DVLD.Contracts.Validators.LicenseClass
{
    public class LicenseClassValidator : AbstractValidator<LicenseClassWriteRequest>
    {
        public LicenseClassValidator()
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
}
