using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class ContaService : IContaService
    {
         private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ContaService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    

        public async Task<IEnumerable<ContaResponseDTO>> ListarTodos()
        {
            var contas= await _context.Contas.ToListAsync();
            return _mapper.Map<IEnumerable<ContaResponseDTO>>(contas);
        }

        public async Task<ContaResponseDTO?> BuscarPorId(int id)   
        {
            var contas = await _context.Contas.FindAsync(id);
            return _mapper.Map<ContaResponseDTO>(contas);
        }

        public async Task<ContaResponseDTO> Criar(ContaResponseDTO contaDto)
        {
            var contas = _mapper.Map<Conta>(contaDto);
            _context.Contas.Add(contas);
            await _context.SaveChangesAsync();
            return _mapper.Map<ContaResponseDTO>(contas);
        }

        public async Task Atualizar(int id, ContaUpdateDTO contaDto)
        {
            var contaExistente = await _context.Contas.FindAsync(id);

            if (contaExistente != null)
            {
                _mapper.Map(contaDto, contaExistente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Deletar(int id)
        {
            var contas = await _context.Contas.FindAsync(id);
            if (contas != null)
            {
                _context.Contas.Remove(contas);
                await _context.SaveChangesAsync();
            }
        }
    }    
}