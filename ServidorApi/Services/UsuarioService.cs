using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ISenhaHasher _senhaHasher;

        public UsuarioService(DataContext context, IMapper mapper, ISenhaHasher senhaHasher)
        {
            _context = context;
            _mapper = mapper;
            _senhaHasher = senhaHasher;
        }

        public async Task<IEnumerable<UsuarioResponseDTO>> ListarTodos()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return _mapper.Map<IEnumerable<UsuarioResponseDTO>>(usuarios);
        }

        public async Task<UsuarioResponseDTO?> BuscarPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> Criar(UsuarioCreateDTO dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.Senha = _senhaHasher.Hash(dto.Senha);
            usuario.DataCriacao = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task Atualizar(int id, UsuarioUpDateDTO usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _mapper.Map(usuarioDto, usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Deletar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}