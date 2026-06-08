using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
 
namespace ServidorApi.Services
{
    public class SaidaService : ISaidaService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
 
        public SaidaService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
 
        public async Task<IEnumerable<SaidaResponseDTO>> ListarTodos()
        {
            var saidas = await _context.Saidas.ToListAsync();
            return _mapper.Map<IEnumerable<SaidaResponseDTO>>(saidas);
        }
 
        public async Task<SaidaResponseDTO?> BuscarPorId(int id)
        {
            var saida = await _context.Saidas.FindAsync(id);
            return _mapper.Map<SaidaResponseDTO>(saida);
        }
 
        public async Task<SaidaResponseDTO> Criar(SaidaResponseDTO saidaDto)
        {
            // Verifica se o usuário existe antes de criar a saída
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.ID == saidaDto.IDUsuario);
 
            if (!usuarioExiste)
                throw new InvalidOperationException("Usuário não encontrado.");
 
            var saida = _mapper.Map<Saida>(saidaDto);
            _context.Saidas.Add(saida);
            await _context.SaveChangesAsync();
            return _mapper.Map<SaidaResponseDTO>(saida);
        }
 
        public async Task<SaidaResponseDTO> Atualizar(int id, SaidaUpdateDTO saidaDto)
        {
            var saida = await _context.Saidas.FindAsync(id);
            if (saida != null)
            {
                _mapper.Map(saidaDto, saida);
                await _context.SaveChangesAsync();
                return _mapper.Map<SaidaResponseDTO>(saida);
            }
            return null!;
        }
 
        public async Task Deletar(int id)
        {
            var saida = await _context.Saidas.FindAsync(id);
            if (saida != null)
            {
                _context.Saidas.Remove(saida);
                await _context.SaveChangesAsync();
            }
        }
 
        public async Task<IEnumerable<SaidaResponseDTO>> ListarComFiltros(
            decimal? valorMinimo, DateTime? dataInicio)
        {
            var query = _context.Saidas.AsQueryable();
 
            if (valorMinimo.HasValue)
                query = query.Where(s => s.ValorSaida >= valorMinimo.Value);
 
            if (dataInicio.HasValue)
                query = query.Where(s => s.DataSaida >= dataInicio.Value);
 
            var lista = await query.ToListAsync();
            return _mapper.Map<IEnumerable<SaidaResponseDTO>>(lista);
        }
    }
}