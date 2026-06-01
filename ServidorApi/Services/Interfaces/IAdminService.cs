// ServidorApi/Services/Interfaces/IAdminService.cs
// SUBSTITUIR o arquivo existente por este

using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface IAdminService
    {
        // Passo 2.1
        Task<AdminDashboardDTO> ObterDashboardAsync();

        // Passo 2.2
        Task<PaginadoDTO<AdminUsuarioListaDTO>> ListarUsuarios(string? busca, int page, int pageSize);
        Task<AdminUsuarioDetalheDTO?> ObterUsuarioPorId(int id);
        Task<bool> AtualizarUsuario(int id, AdminUsuarioUpdateDTO dto);
        Task<bool> AlterarStatusAtivo(int id, bool ativo); // Implementação parcial até adicionar campo Ativo
        Task<bool> ExcluirUsuario(int id);

        // Passo 2.3
        Task<PaginadoDTO<TransacaoAdminDTO>> ListarTransacoes(TransacaoFiltroDTO filtro);
    }
}