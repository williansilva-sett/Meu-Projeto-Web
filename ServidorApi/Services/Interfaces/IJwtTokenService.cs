using ServidorApi.Models;

namespace ServidorApi.Services.Interfaces
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario);
    }
}