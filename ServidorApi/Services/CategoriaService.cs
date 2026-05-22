using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class CategoriaService : ICategoriaService   
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CategoriaService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoriaResponseDTO>> ListarTodos()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
        }

        public async Task<CategoriaResponseDTO?> BuscarPorId(int id)
        {
            var categorias = await _context.Categorias.FindAsync(id);
            return _mapper.Map<CategoriaResponseDTO>(categorias);
        }

        public async Task<CategoriaResponseDTO> Criar(CategoriaResponseDTO categoriaDto)
        {
            var categorias = _mapper.Map<Categoria>(categoriaDto);
            _context.Categorias.Add(categorias);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoriaResponseDTO>(categorias);
        }

        public async Task Atualizar(int id, CategoriaResponseDTO categoriaDto)
        {
            var categorias = await _context.Categorias.FindAsync(id);
            if (categorias != null)
            {
                _mapper.Map(categoriaDto, categorias);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Deletar(int id)
        {
            var categorias = await _context.Categorias.FindAsync(id);
            if (categorias != null)
            {
                _context.Categorias.Remove(categorias);
                await _context.SaveChangesAsync();
            }
        }
    }
}