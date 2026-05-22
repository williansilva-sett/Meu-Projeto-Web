namespace ServidorApi.DTOs
{
    public class UsuarioAlterarSenhaDTO
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}