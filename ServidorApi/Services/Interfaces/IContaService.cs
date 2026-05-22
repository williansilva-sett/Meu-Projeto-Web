using ServidorApi.DTOs;
using ServidorApi.Models;

namespace ServidorApi.Services.Interfaces
{
    public interface IContaService
    {
        Task<IEnumerable<ContaResponseDTO>> ListarTodos();
        Task<ContaResponseDTO?> BuscarPorId(int id);
        Task<ContaResponseDTO> Criar(ContaResponseDTO contaDto);
        Task Atualizar(int id, ContaUpdateDTO contaDto);
        Task Deletar(int id);
    }
}