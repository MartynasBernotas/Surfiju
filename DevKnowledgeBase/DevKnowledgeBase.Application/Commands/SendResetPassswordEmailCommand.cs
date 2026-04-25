using DevKnowledgeBase.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DevKnowledgeBase.Application.Commands
{
    public record SendResetPassswordEmailCommand(string userId, string email, string token) : IRequest<bool>;

    public class SendResetPassswordEmailHandler : IRequestHandler<SendResetPassswordEmailCommand, bool>
    {
        private readonly IEmailService _emailService;
        private readonly string _frontendBaseUrl;

        public SendResetPassswordEmailHandler(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _frontendBaseUrl = configuration["AppSettings:FrontendBaseUrl"] ?? "https://localhost:7063";
        }

        public async Task<bool> Handle(SendResetPassswordEmailCommand request, CancellationToken cancellationToken)
        {
            var resetLink = $"{_frontendBaseUrl}/reset-password?userId={request.userId}&token={Uri.EscapeDataString(request.token)}";
            await _emailService.SendEmailAsync(request.email, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
            return true;
        }
    }
}
