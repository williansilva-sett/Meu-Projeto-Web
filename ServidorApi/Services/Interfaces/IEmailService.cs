namespace ServidorApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarAsync(string destinatario, string assunto, string corpoHtml);
    }
}