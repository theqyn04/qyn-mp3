using System.Net.Mail;
using System.Net;

namespace qyn_mp3.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
            {
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"])
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: smtpSettings["FromEmail"],
                    to: email,
                    subject,
                    message
                ));
        }
    }

}
