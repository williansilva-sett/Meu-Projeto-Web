using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface ISaidaService
    {
        Task<IEnumerable<SaidaResponseDTO>> ListarTodos();
        Task<SaidaResponseDTO?> BuscarPorId(int id);
        Task<SaidaResponseDTO> Criar(SaidaResponseDTO entradasDto);
        Task <SaidaResponseDTO> Atualizar(int id, SaidaUpdateDTO entradasDto);
        Task Deletar(int id);
        Task<IEnumerable<SaidaResponseDTO>> ListarComFiltros(decimal? valorMinimo, DateTime? dataInicio);
    }
}