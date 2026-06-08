namespace ServidorApi.DTOs
{
    public class SaidaResponseDTO
    {
        public int IDSaida { get; set; }
 
        public decimal ValorSaida { get; set; }
 
        public DateTime DataSaida { get; set; } = DateTime.Now;
 
        // IDConta removido — saída agora pertence ao Usuario diretamente
        public int IDUsuario { get; set; }
 
        public int IDCategoria { get; set; }
    }
}