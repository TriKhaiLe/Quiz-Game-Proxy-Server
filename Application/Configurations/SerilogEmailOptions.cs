namespace QuizGameServer.Application.Configurations
{
    public class SerilogEmailOptions
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string MailServer { get; set; }
        public int Port { get; set; } = 587;
        public string EmailSubject { get; set; }
        public bool EnableSsl { get; set; } = true;

        public EmailNetworkCredential NetworkCredentials { get; set; }
    }

    public class EmailNetworkCredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
