using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface IEntradaService
    {
        Task<IEnumerable<EntradasResponseDTO>> ListarTodos();
        Task<EntradasResponseDTO?> BuscarPorId(int id);
        Task<EntradasResponseDTO> Criar(EntradasResponseDTO entradasDto);
        Task <EntradasResponseDTO> Atualizar(int id, EntradasUpdateDTO entradasDto);
        Task Deletar(int id);
        Task<IEnumerable<EntradasResponseDTO>> ListarComFiltros(decimal? valorMinimo, DateTime? dataInicio);
    }
}