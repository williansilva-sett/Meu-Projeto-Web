using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto) =>
            await LoginInternoAsync(dto, exigeAdmin: false);

        [HttpPost("login-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequestDTO dto) =>
            await LoginInternoAsync(dto, exigeAdmin: true);

        private async Task<IActionResult> LoginInternoAsync(LoginRequestDTO dto, bool exigeAdmin)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                return BadRequest("Email e senha são obrigatórios.");

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower().Trim());

            // PROTEÇÃO CONTRA TIMING ATTACK:
            // Simula o trabalho do BCrypt mesmo quando o usuário não existe
            // Evita que um atacante descubra emails válidos medindo o tempo de resposta
            if (usuario is null)
            {
                _senhaHasher.Verify("dummy", BCrypt.Net.BCrypt.HashPassword("dummy"));
                return Unauthorized("Email ou senha inválidos.");
            }

            // Verifica se a conta está bloqueada por excesso de tentativas
            if (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte > DateTime.UtcNow)
            {
                var minutosRestantes = (int)(usuario.BloqueadoAte.Value - DateTime.UtcNow).TotalMinutes + 1;
                return StatusCode(429, $"Conta bloqueada. Tente novamente em {minutosRestantes} minuto(s).");
            }

            // Verifica a senha
            if (!_senhaHasher.Verify(dto.Senha, usuario.Senha))
            {
                usuario.TentativasLogin++;

                // Bloqueia após 5 tentativas por 15 minutos
                if (usuario.TentativasLogin >= 5)
                {
                    usuario.BloqueadoAte   = DateTime.UtcNow.AddMinutes(15);
                    usuario.TentativasLogin = 0;
                }

                await _context.SaveChangesAsync();
                return Unauthorized("Email ou senha inválidos.");
            }

            // Login bem-sucedido — zera tentativas e remove bloqueio
            usuario.TentativasLogin = 0;
            usuario.BloqueadoAte   = null;
            await _context.SaveChangesAsync();

            if (exigeAdmin && usuario.Tipo != TipoUsuario.Admin)
                return Forbid();

            var (token, expiraEm) = _jwtTokenService.GerarToken(usuario);

            return Ok(new LoginResponseDTO
            {
                Token    = token,
                ExpiraEm = expiraEm,
                Nome     = usuario.Nome,
                Tipo     = usuario.Tipo.ToString()
            });
        }
    }
}
