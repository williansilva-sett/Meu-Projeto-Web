namespace ServidorApi.Configuration
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string EmailRemetente { get; set; } = string.Empty;
        public string NomeRemetente { get; set; } = string.Empty;
        public string SenhaApp { get; set; } = string.Empty;
    }
}