using Microsoft.AspNetCore.Identity.UI.Services;

namespace SchoolManegementNew.Services.Email
{
    public class IdentityMailSender : IEmailSender
    {
        private readonly EmailService _emailService;

        public IdentityMailSender(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _emailService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
