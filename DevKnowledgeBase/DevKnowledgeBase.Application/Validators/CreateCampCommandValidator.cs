using DevKnowledgeBase.Application.Commands;
using FluentValidation;

namespace DevKnowledgeBase.Application.Validators
{
    public class CreateCampCommandValidator : AbstractValidator<CreateCampCommand>
    {
        public CreateCampCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Camp name is required.")
                .MaximumLength(100).WithMessage("Camp name cannot exceed 100 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThan(DateTime.UtcNow.Date).WithMessage("Start date must be in the future.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Max participants must be greater than 0.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(500).WithMessage("Location cannot exceed 500 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }
}
