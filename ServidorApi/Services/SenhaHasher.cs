using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class SenhaHasher : ISenhaHasher
    {
    public string Hash(string senhaEmTexto) =>
    BCrypt.Net.BCrypt.HashPassword(senhaEmTexto);

    public bool Verify(string senhaEmTexto, string hashArmazenado) =>
    BCrypt.Net.BCrypt.Verify(senhaEmTexto, hashArmazenado);
    }
}