using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ISenhaHasher _senhaHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            DataContext context,
            ISenhaHasher senhaHasher,
            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _senhaHasher = senhaHasher;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto) =>
            await LoginInternoAsync(dto, exigeAdmin: false);

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequestDTO dto) =>
            await LoginInternoAsync(dto, exigeAdmin: true);

        private async Task<IActionResult> LoginInternoAsync(LoginRequestDTO dto, bool exigeAdmin)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                return BadRequest("Email e senha são obrigatórios.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !_senhaHasher.Verify(dto.Senha, usuario.Senha))
                return Unauthorized("Email ou senha inválidos.");

            if (exigeAdmin && usuario.Tipo != TipoUsuario.Admin)
                return Forbid();

            var (token, expiraEm) = _jwtTokenService.GerarToken(usuario);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                ExpiraEm = expiraEm,
                Nome = usuario.Nome,
                Tipo = usuario.Tipo.ToString()
            });
        }
    }
}
