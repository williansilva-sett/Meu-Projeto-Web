using ServidorApi.Models;
 
namespace ServidorApi.Services.Interfaces
{
    public interface IJwtTokenService
    {
        // Recebe a Conta de sistema (que contém email, tipo, id)
        // e os dados pessoais via navegação conta.Usuario
        (string Token, DateTime ExpiraEm) GerarToken(Conta conta);
    }
}
 