using DevKnowledgeBase.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DevKnowledgeBase.Application.Commands
{
    public record SendEmailConfirmationCommand(string userId, string email, string token) : IRequest<bool>;

    public class SendEmailConfirmationHandler : IRequestHandler<SendEmailConfirmationCommand, bool>
    {
        private readonly IEmailService _emailService;
        private readonly string _frontendBaseUrl;

        public SendEmailConfirmationHandler(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _frontendBaseUrl = configuration["AppSettings:FrontendBaseUrl"] ?? "https://localhost:7063";
        }

        public async Task<bool> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var confirmationLink = $"{_frontendBaseUrl}/confirm-email?userId={request.userId}&token={request.token}";
            await _emailService.SendEmailAsync(request.email, "Confirm Your Email", $"Click <a href='{confirmationLink}'>here</a> to confirm your email.");
            return true;
        }
    }
}
