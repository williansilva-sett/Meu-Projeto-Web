namespace ServidorApi.DTOs
{
    public class EntradasResponseDTO
    {
        public int IDEntrada { get; set; }

        public string Descricao {get; set; } = string.Empty;

        public decimal ValorEntrada {get; set; }

        public DateTime Data {get; set; } = DateTime.Now;

        public int IDConta { get; set; }

        public int IDCategoria { get; set; }
    }
}