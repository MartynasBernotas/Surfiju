using DevKnowledgeBase.Application.Commands;
using FluentValidation;

namespace DevKnowledgeBase.Application.Validators
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.CampId)
                .NotEmpty().WithMessage("Camp ID is required.");

            RuleFor(x => x.Participants)
                .GreaterThan(0).WithMessage("Number of participants must be at least 1.")
                .LessThanOrEqualTo(50).WithMessage("Number of participants cannot exceed 50.");
        }
    }
}
