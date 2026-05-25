namespace ServidorApi.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalUsuarios   { get; set; }
        public int TotalContas     { get; set; }
        public int TotalEntradas   { get; set; }
        public decimal ValorEntradas { get; set; }
        public int TotalSaidas     { get; set; }
        public decimal ValorSaidas { get; set; }
        public decimal SaldoGeral  { get; set; }
        public DateTime DataConsulta { get; set; }
    }
}