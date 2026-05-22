using ServidorApi.DTOs;

namespace ServidorApi.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioResponseDTO>> ListarTodos();
        Task<UsuarioResponseDTO?> BuscarPorId(int id); // Adicione este
        Task<UsuarioResponseDTO> Criar(UsuarioCreateDTO usuarioDto); // Adicione este
        Task Atualizar(int id, UsuarioUpDateDTO usuarioDto); // Adicione este
        Task Deletar(int id); // Adicione este
    }
}