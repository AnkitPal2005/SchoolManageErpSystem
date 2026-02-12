using MimeKit;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace SchoolManegementNew.Services.Email
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail,string subject,string bodyHtml, byte[]? attachmentBytes = null, string? attachmentName = null)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                "School Management System",
                "ap7023278@gmail.com"
                ));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = bodyHtml
            };
            if (attachmentBytes != null && attachmentName != null)
            {
                builder.Attachments.Add(
                    attachmentName,
                    attachmentBytes
                    );
            }
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls
                );
            await smtp.AuthenticateAsync(
                "ap7023278@gmail.com",
                "glrtmlivwsyvjguh"
                );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
