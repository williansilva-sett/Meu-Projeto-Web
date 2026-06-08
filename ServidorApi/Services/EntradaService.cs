using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
 
namespace ServidorApi.Services
{
    public class EntradaService : IEntradaService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
 
        public EntradaService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
 
        public async Task<IEnumerable<EntradasResponseDTO>> ListarTodos()
        {
            var entradas = await _context.Entrada.ToListAsync();
            return _mapper.Map<IEnumerable<EntradasResponseDTO>>(entradas);
        }
 
        public async Task<EntradasResponseDTO?> BuscarPorId(int id)
        {
            var entrada = await _context.Entrada.FindAsync(id);
            return _mapper.Map<EntradasResponseDTO>(entrada);
        }
 
        public async Task<EntradasResponseDTO> Criar(EntradasResponseDTO entradasDto)
        {
            // Verifica se o usuário existe antes de criar a entrada
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.ID == entradasDto.IDUsuario);
 
            if (!usuarioExiste)
                throw new InvalidOperationException("Usuário não encontrado.");
 
            var entrada = _mapper.Map<Entradas>(entradasDto);
            _context.Entrada.Add(entrada);
            await _context.SaveChangesAsync();
            return _mapper.Map<EntradasResponseDTO>(entrada);
        }
 
        public async Task<EntradasResponseDTO> Atualizar(int id, EntradasUpdateDTO entradasDto)
        {
            var entrada = await _context.Entrada.FindAsync(id);
            if (entrada != null)
            {
                _mapper.Map(entradasDto, entrada);
                await _context.SaveChangesAsync();
                return _mapper.Map<EntradasResponseDTO>(entrada);
            }
            return null!;
        }
 
        public async Task Deletar(int id)
        {
            var entrada = await _context.Entrada.FindAsync(id);
            if (entrada != null)
            {
                _context.Entrada.Remove(entrada);
                await _context.SaveChangesAsync();
            }
        }
 
        public async Task<IEnumerable<EntradasResponseDTO>> ListarComFiltros(
            decimal? valorMinimo, DateTime? dataInicio)
        {
            var query = _context.Entrada.AsQueryable();
 
            if (valorMinimo.HasValue)
                query = query.Where(e => e.ValorEntrada >= valorMinimo.Value);
 
            if (dataInicio.HasValue)
                query = query.Where(e => e.Data >= dataInicio.Value);
 
            var lista = await query.ToListAsync();
            return _mapper.Map<IEnumerable<EntradasResponseDTO>>(lista);
        }
    }
}