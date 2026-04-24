using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;

namespace ServidorApi.Controllers
{
    [Route("api/[controller]")] // Define a rota como api/usuarios
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        // O ASP.NET injeta o Banco e o Mapeador automaticamente aqui
        public UsuariosController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            
            // Converte a lista de Entidades para uma lista de DTOs
            var usuariosDto = _mapper.Map<List<UsuarioResponseDTO>>(usuarios);
            
            return Ok(usuariosDto);
        }

        // POST: api/usuarios (Cadastro)
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDTO>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var usuarioDto = _mapper.Map<UsuarioResponseDTO>(usuario);

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.UsuarioID }, usuarioDto);
        }
    }
}