using ServidorApi.DTOs;
 
namespace ServidorApi.Services.Interfaces
{
    public interface IMetaService
    {
        // Lista todas as metas de um usuário
        Task<IEnumerable<MetaResponseDTO>> ListarPorUsuario(int usuarioId);
 
        // Busca uma meta pelo ID
        Task<MetaResponseDTO?> BuscarPorId(int id);
 
        // Cria uma nova meta
        Task<MetaResponseDTO> Criar(MetaCreateDTO dto);
 
        // Atualiza nome, descrição, valor alvo e data limite
        Task<bool> Atualizar(int id, MetaUpdateDTO dto);
 
        // Atualiza o valor atual (progresso)
        Task<bool> AtualizarProgresso(int id, MetaProgressoDTO dto);
 
        // Atualiza o status (EmAndamento, Concluida, Cancelada)
        Task<bool> AtualizarStatus(int id, MetaStatusDTO dto);
 
        // Deleta uma meta
        Task<bool> Deletar(int id);
    }
}