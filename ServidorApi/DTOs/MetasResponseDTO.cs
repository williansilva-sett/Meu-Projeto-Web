namespace ServidorApi.DTOs
{
    // Resposta — GET /api/meta e GET /api/meta/{id}
    public class MetaResponseDTO
    {
        public int ID { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal ValorAlvo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public string Status { get; set; } = string.Empty;  // "EmAndamento", "Concluida", "Cancelada"
        public int IDUsuario { get; set; }
 
        // Percentual de progresso calculado — útil para barra de progresso no frontend
        public decimal Progresso => ValorAlvo > 0
            ? Math.Min(Math.Round(ValorAtual / ValorAlvo * 100, 1), 100)
            : 0;
    }
 
    // Criação — POST /api/meta
    public class MetaCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal ValorAlvo { get; set; }
        public decimal ValorAtual { get; set; } = 0;
        public DateTime DataInicio { get; set; } = DateTime.Now;
        public DateTime? DataLimite { get; set; }
        public int IDUsuario { get; set; }
    }
 
    // Atualização completa — PUT /api/meta/{id}
    public class MetaUpdateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal ValorAlvo { get; set; }
        public DateTime? DataLimite { get; set; }
    }
 
    // Atualização do valor atual — PATCH /api/meta/{id}/progresso
    // Usado quando o usuário registra um aporte na meta
    public class MetaProgressoDTO
    {
        public decimal ValorAtual { get; set; }
    }
 
    // Atualização do status — PATCH /api/meta/{id}/status
    public class MetaStatusDTO
    {
        public string Status { get; set; } = string.Empty; // "EmAndamento", "Concluida", "Cancelada"
    }
}