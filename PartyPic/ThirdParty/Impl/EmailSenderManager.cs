using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PartyPic.ThirdParty;
using System.Linq;

public class EmailSenderManager : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSenderManager(IConfiguration config)
    {
        _config = config;
    }

    public void SendEmail(string toName, string toEmail, string subject, string htmlBody, string imagePath)
    {
        var message = new MimeMessage();

        var from = new MailboxAddress("PartyPic Admin", _config.GetValue<string>("EmailFromAdmin"));
        message.From.Add(from);

        var to = new MailboxAddress(toName, toEmail);
        message.To.Add(to);

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };

        if (!string.IsNullOrEmpty(imagePath))
        {
            var image = bodyBuilder.LinkedResources.Add(imagePath);
            image.ContentId = "myImage";
        }

        message.Body = bodyBuilder.ToMessageBody();

        foreach (var body in message.BodyParts.OfType<TextPart>())
            body.ContentTransferEncoding = ContentEncoding.Base64;

        using (var client = new SmtpClient())
        {
            client.CheckCertificateRevocation = false;
            client.Connect(_config.GetValue<string>("SMTPServer"), 2525, SecureSocketOptions.StartTls);
            client.Authenticate(_config.GetValue<string>("SMTPUser"), _config.GetValue<string>("SMTPPassword"));

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
