// ServidorApi/DTOs/TransacaoAdminDTO.cs

namespace ServidorApi.DTOs
{
    public class TransacaoAdminDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public int IdConta { get; set; }
        public string NomeConta { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
        public string NomeCategoria { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
    }

    public class TransacaoFiltroDTO
    {
        public string Tipo { get; set; } = "todos";
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? IdConta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}