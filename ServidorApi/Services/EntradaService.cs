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
            var entradas = await _context.Entrada.FindAsync(id);
            return _mapper.Map<EntradasResponseDTO>(entradas);
        }

        public async Task<EntradasResponseDTO> Criar(EntradasResponseDTO entradasDto)
        {
            var entradas = _mapper.Map<Entradas>(entradasDto);
            _context.Entrada.Add(entradas);
            await _context.SaveChangesAsync();
            return _mapper.Map<EntradasResponseDTO>(entradas);
        }

        public async Task <EntradasResponseDTO> Atualizar(int id, EntradasUpdateDTO entradasDto)
        {
            var entradas = await _context.Entrada.FindAsync(id);
            if (entradas != null)
            {
                _mapper.Map(entradasDto, entradas);
                await _context.SaveChangesAsync();
                return _mapper.Map<EntradasResponseDTO>(entradas);
            }
            return null!;
        }

        public async Task Deletar(int id)
        {
            var entradas = await _context.Entrada.FindAsync(id);
            if (entradas != null)
            {
                _context.Entrada.Remove(entradas);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EntradasResponseDTO>> ListarComFiltros(decimal? valorMinimo, DateTime? dataInicio)
        {
            var lista = await _context.Entrada.ToListAsync(); // Use Entrada, não tb_entrada
            return _mapper.Map<IEnumerable<EntradasResponseDTO>>(lista);
        }
    }    
}