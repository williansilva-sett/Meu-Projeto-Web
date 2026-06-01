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
        Task<bool> AlterarSenha(int usuarioId, UsuarioAlterarSenhaDTO dto); // Verifica a senha atual antes de permitir a alteração
    }
}