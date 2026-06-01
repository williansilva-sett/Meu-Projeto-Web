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

        public async Task<bool> AlterarSenha(int usuarioId, UsuarioAlterarSenhaDTO dto)
        {
            // Busca o usuário pelo ID que vem do token JWT (não do body)
            // Evita que um usuário altere a senha de outro
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario is null) return false;

            // Valida que a senha atual digitada bate com o hash no banco
            // Sem essa verificação, qualquer um com o token poderia trocar a senha
            if (!_senhaHasher.Verify(dto.SenhaAtual, usuario.Senha))
                return false;

            // Aplica o hash na nova senha e salva
            usuario.Senha = _senhaHasher.Hash(dto.NovaSenha);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}