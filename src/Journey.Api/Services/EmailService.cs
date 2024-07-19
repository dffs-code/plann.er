using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;

    public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _smtpUser = smtpUser;
        _smtpPass = smtpPass;
    }

    public void SendEmail(string toEmail, string subject, string body, bool isBodyHtml = false)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };
            mailMessage.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMessage);
            }

            Console.WriteLine("Email sent succesfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong: {ex.Message}");
        }
    }
}
