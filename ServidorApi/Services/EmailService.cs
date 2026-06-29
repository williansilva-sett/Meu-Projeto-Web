// Requer o pacote MailKit:
//   dotnet add package MailKit

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ServidorApi.Configuration;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarAsync(string destinatario, string assunto, string corpoHtml)
        {
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress(_settings.NomeRemetente, _settings.EmailRemetente));
            mensagem.To.Add(MailboxAddress.Parse(destinatario));
            mensagem.Subject = assunto;
            mensagem.Body = new TextPart("html") { Text = corpoHtml };

            using var cliente = new SmtpClient();
            await cliente.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await cliente.AuthenticateAsync(_settings.EmailRemetente, _settings.SenhaApp);
            await cliente.SendAsync(mensagem);
            await cliente.DisconnectAsync(true);
        }
    }
}