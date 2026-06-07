using DVLD.Contracts.Requests.TestAppointment;
using FluentValidation;

namespace DVLD.Contracts.Validators.TestAppointment
{
    public class UpdateTestAppointmentRequestValidator : AbstractValidator<UpdateTestAppointmentRequest>
    {
        public UpdateTestAppointmentRequestValidator()
        {
            RuleFor(x => x.AppointmentDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");
        }
    }
}
