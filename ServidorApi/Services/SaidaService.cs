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
            var saidas = await _context.Saidas.FindAsync(id);
            return _mapper.Map<SaidaResponseDTO>(saidas);
        }

        public async Task<SaidaResponseDTO> Criar(SaidaResponseDTO saidaDto)
        {
            var saidas = _mapper.Map<Saida>(saidaDto);
            _context.Saidas.Add(saidas);
            await _context.SaveChangesAsync();
            return _mapper.Map<SaidaResponseDTO>(saidas);
        }

        public async Task <SaidaResponseDTO> Atualizar(int id, SaidaUpdateDTO saidaDto)
        {
            var saidas = await _context.Saidas.FindAsync(id);
            if (saidas != null)
            {
                _mapper.Map(saidaDto, saidas);
                await _context.SaveChangesAsync();
                return _mapper.Map<SaidaResponseDTO>(saidas);
            }
            return null!;
        }

        public async Task Deletar(int id)
        {
            var saidas = await _context.Saidas.FindAsync(id);
            if (saidas != null)
            {
                _context.Saidas.Remove(saidas);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SaidaResponseDTO>> ListarComFiltros(decimal? valorMinimo, DateTime? dataInicio)
        {
            var lista = await _context.Saidas.ToListAsync(); // Use Entrada, não tb_entrada
            return _mapper.Map<IEnumerable<SaidaResponseDTO>>(lista);
        }
    }    
}