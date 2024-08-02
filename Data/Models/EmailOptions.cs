namespace ndisforms.Data.Models
{
    public class EmailOptions

    {
        public bool Active { get; set; }
        public const string Email = "Email";

        public string SmtpServer { get; set; }

        public string SmtpPort { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

    }
}
