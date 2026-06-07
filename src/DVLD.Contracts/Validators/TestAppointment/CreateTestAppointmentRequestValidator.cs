using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.TestAppointment;
using FluentValidation;

namespace DVLD.Contracts.Validators.TestAppointment
{
    public class CreateTestAppointmentRequestValidator : AbstractValidator<CreateTestAppointmentRequest>
    {
        public CreateTestAppointmentRequestValidator()
        {
            RuleFor(x => x.LocalDrivingLicenseApplicationId)
                .GreaterThan(0).WithMessage("Local driving license application is required.");

            RuleFor(x => x.TestTypeId)
                .Must(value => Enum.IsDefined(typeof(enTestType), value)).WithMessage("Invalid test type.");

            RuleFor(x => x.AppointmentDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");
        }
    }
}
