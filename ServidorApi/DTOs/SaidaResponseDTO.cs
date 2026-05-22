namespace ServidorApi.DTOs
{
    public class SaidaResponseDTO
    {
        public int IDSaida {get; set; }

        public decimal ValorSaida {get; set; }

        public DateTime DataSaida {get; set; } = DateTime.Now;

        public int IDConta { get; set; }

        public int IDCategoria { get; set; }
    }
}