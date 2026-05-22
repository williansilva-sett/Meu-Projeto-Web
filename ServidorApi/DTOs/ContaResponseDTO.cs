namespace ServidorApi.DTOs
{
    public class ContaResponseDTO
    {
        public int IDConta { get; set; }

        public string NomeConta { get; set; } = string.Empty;

        public bool Ativa { get; set; }
        
        public int UsuarioID { get; set; }
    }
}