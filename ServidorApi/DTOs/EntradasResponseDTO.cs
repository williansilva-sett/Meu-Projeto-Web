namespace ServidorApi.DTOs
{
    public class EntradasResponseDTO
    {
        public int IDEntrada { get; set; }
 
        public string Descricao { get; set; } = string.Empty;
 
        public decimal ValorEntrada { get; set; }
 
        public DateTime Data { get; set; } = DateTime.Now;
 
        // IDConta removido — entrada agora pertence ao Usuario diretamente
        public int IDUsuario { get; set; }
 
        public int IDCategoria { get; set; }
    }
}