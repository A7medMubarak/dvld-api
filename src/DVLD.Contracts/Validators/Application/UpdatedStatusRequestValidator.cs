using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Requests.Application;
using FluentValidation;

namespace DVLD.Contracts.Validators.Application
{
    public class UpdatedStatusRequestValidator : AbstractValidator<UpdatedStatusRequest>
    {
        public UpdatedStatusRequestValidator()
        {
            RuleFor(x => x.NewStatus)
                .InclusiveBetween((short)enApplicationStatus.New, (short)enApplicationStatus.Completed)
                .WithMessage("Invalid application status.");
        }
    }
}
