using DevKnowledgeBase.Application.Commands;
using FluentValidation;

namespace DevKnowledgeBase.Application.Validators
{
    public class CreateSurfSpotCommandValidator : AbstractValidator<CreateSurfSpotCommand>
    {
        public CreateSurfSpotCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(x => x.Dto.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Dto.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }
}
