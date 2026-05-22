namespace ServidorApi.Services.Interfaces
{
    public interface ISenhaHasher
    {
    string Hash(string senhaEmTexto);
    bool Verify(string senhaEmTexto, string hashArmazenado);
    }
}