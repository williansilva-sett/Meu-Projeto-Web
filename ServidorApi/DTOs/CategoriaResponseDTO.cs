namespace ServidorApi.DTOs
{
    public class CategoriaResponseDTO
    {
        public int IDCategoria { get; set; }

        public string categoria { get; set; } = string.Empty;
        
        public string Tipo { get; set; } = string.Empty;

        public int? IDUsuario { get; set; }
    }
}