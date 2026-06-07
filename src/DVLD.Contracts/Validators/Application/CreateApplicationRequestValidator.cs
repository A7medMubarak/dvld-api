using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.Application;
using FluentValidation;

namespace DVLD.Contracts.Validators.Application
{
    public class CreateApplicationRequestValidator : AbstractValidator<CreateApplicationRequest>
    {
        public CreateApplicationRequestValidator()
        {
            RuleFor(x => x.ApplicantPersonId)
                .GreaterThan(0).WithMessage("Applicant person is required.");

            RuleFor(x => x.ApplicationTypeId)
                .GreaterThan(0).WithMessage("Application type is required.")
                .Must(value => Enum.IsDefined(typeof(enApplicationType), value)).WithMessage("Invalid application type.");

            RuleFor(x => x.ApplicationStatus)
                .GreaterThan((byte)0).WithMessage("Application status is required.")
                .Must(value => Enum.IsDefined(typeof(enApplicationStatus), value)).WithMessage("Invalid application status.");

            RuleFor(x => x.PaidFees)
                .GreaterThanOrEqualTo(5).WithMessage("Paid fees must be at least 5.");
        }
    }
}
