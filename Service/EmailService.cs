using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace BookZoneAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var fromAddress = new MailAddress(emailSettings["FromEmail"], emailSettings["FromName"]);
            var toAddress = new MailAddress(toEmail);
            var fromPassword = emailSettings["Password"];
            var username = emailSettings["Username"];
            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"]);

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(username, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
