using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public record SendEmailConfirmationCommand(string userId, string email, string token) : IRequest<bool>;

    public class SendEmailConfirmationHandler : IRequestHandler<SendEmailConfirmationCommand, bool>
    {
        private readonly IEmailService _emailService;
        public SendEmailConfirmationHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var confirmationLink = $"https://localhost:7063/confirm-email?userId={request.userId}&token={request.token}";

            // Send email
            await _emailService.SendEmailAsync(request.email, "Confirm Your Email", $"Click <a href='{confirmationLink}'>here</a> to confirm your email.");

            return true;
        }
    }
}
