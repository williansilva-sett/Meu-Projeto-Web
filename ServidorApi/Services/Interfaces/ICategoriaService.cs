using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaResponseDTO>> ListarTodos();
        Task<CategoriaResponseDTO?> BuscarPorId(int id); // Adicione este
        Task<CategoriaResponseDTO> Criar(CategoriaResponseDTO categoriaDto); // Adicione este
        Task Atualizar(int id, CategoriaResponseDTO categoriaDto); // Adicione este
        Task Deletar(int id); // Adicione este
    }
}