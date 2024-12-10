namespace PartyPic.ThirdParty
{
    public interface IEmailSender
    {
        void SendEmail(string toName, string toEmail, string subject, string htmlBody, string imagePath);
    }
}
