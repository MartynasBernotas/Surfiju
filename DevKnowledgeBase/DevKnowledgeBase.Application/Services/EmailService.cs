using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace DevKnowledgeBase.Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["Email:SmtpHost"])
            {
                Port = int.Parse(_configuration["Email:SmtpPort"]),
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:SenderEmail"], _configuration["Email:SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
