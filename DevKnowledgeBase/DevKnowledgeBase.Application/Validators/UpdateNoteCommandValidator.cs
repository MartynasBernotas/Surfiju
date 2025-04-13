using DevKnowledgeBase.Application.Commands;
using FluentValidation;

namespace DevKnowledgeBase.Application.Validators
{
    public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
    {
        public UpdateNoteCommandValidator() 
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters long.");
        }
    }
}
