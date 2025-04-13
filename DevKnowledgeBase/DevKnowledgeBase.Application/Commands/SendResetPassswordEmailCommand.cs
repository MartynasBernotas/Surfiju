using DevKnowledgeBase.Application.Services;
using MediatR;

namespace DevKnowledgeBase.Application.Commands
{
    public record SendResetPassswordEmailCommand(string userId, string email, string token) : IRequest<bool>;
    public class SendResetPassswordEmailHandler : IRequestHandler<SendResetPassswordEmailCommand, bool>
    {
        private readonly IEmailService _emailService;
        public SendResetPassswordEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendResetPassswordEmailCommand request, CancellationToken cancellationToken)
        {
            var resetLink = $"https://localhost:7063/reset-password?userId={request.userId}&token={Uri.EscapeDataString(request.token)}";
            await _emailService.SendEmailAsync(request.email, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return true;
        }
    }
}
