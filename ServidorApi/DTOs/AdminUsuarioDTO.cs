// ServidorApi/DTOs/AdminUsuarioDTOs.cs
// CRIAR esse arquivo na pasta DTOs/

namespace ServidorApi.DTOs
{
    // Resposta da listagem paginada — GET /api/admin/usuarios
    public class AdminUsuarioListaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Usuario" ou "Admin"
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    // Resposta do detalhe — GET /api/admin/usuarios/{id}
    public class AdminUsuarioDetalheDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }

        // Contas vinculadas ao usuário
        public List<AdminContaResumoDTO> Contas { get; set; } = new();
    }

    // Resumo de conta usado dentro do detalhe do usuário
    public class AdminContaResumoDTO
    {
        public int IDConta { get; set; }
        public string NomeConta { get; set; } = string.Empty;
        public bool Ativa { get; set; }
    }

    // Corpo do PUT /api/admin/usuarios/{id}
    public class AdminUsuarioUpdateDTO
    {
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // Envelope de resposta paginada — reutilizável
    public class PaginadoDTO<T>
    {
        public List<T> Itens { get; set; } = new();
        public int TotalItens { get; set; }
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
    }
}