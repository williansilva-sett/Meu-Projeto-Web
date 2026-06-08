namespace ServidorApi.DTOs
{
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Telefone { get; set; } = string.Empty;
 
        // Email vem da Conta de sistema via mapeamento no MappingProfile
        public string Email { get; set; } = string.Empty;
 
        // DataCriacao agora vem da Conta (quando a conta foi criada)
        public DateTime DataCriacao { get; set; }
    }
}