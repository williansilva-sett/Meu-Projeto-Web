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
 
            // Busca a conta de sistema pelo email
            // Include(c => c.Usuario) carrega os dados pessoais junto
            // necessário para retornar o Nome no token e no response
            var conta = await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Email == dto.Email.ToLower().Trim());
 
            // PROTEÇÃO CONTRA TIMING ATTACK:
            // Simula o trabalho do BCrypt mesmo quando a conta não existe
            // Evita que um atacante descubra emails válidos medindo o tempo de resposta
            if (conta is null)
            {
                _senhaHasher.Verify("dummy", BCrypt.Net.BCrypt.HashPassword("dummy"));
                return Unauthorized("Email ou senha inválidos.");
            }
 
            // Verifica se a conta está bloqueada por excesso de tentativas
            if (conta.BloqueadoAte.HasValue && conta.BloqueadoAte > DateTime.UtcNow)
            {
                var minutosRestantes = (int)(conta.BloqueadoAte.Value - DateTime.UtcNow).TotalMinutes + 1;
                return StatusCode(429, $"Conta bloqueada. Tente novamente em {minutosRestantes} minuto(s).");
            }
 
            // Verifica a senha contra o hash armazenado na Conta
            if (!_senhaHasher.Verify(dto.Senha, conta.Senha))
            {
                conta.TentativasLogin++;
 
                // Bloqueia após 5 tentativas por 15 minutos
                if (conta.TentativasLogin >= 5)
                {
                    conta.BloqueadoAte    = DateTime.UtcNow.AddMinutes(15);
                    conta.TentativasLogin = 0;
                }
 
                await _context.SaveChangesAsync();
                return Unauthorized("Email ou senha inválidos.");
            }
 
            // Login bem-sucedido — zera tentativas e remove bloqueio
            conta.TentativasLogin = 0;
            conta.BloqueadoAte    = null;
            await _context.SaveChangesAsync();
 
            // Verifica o tipo DEPOIS da validação de senha
            // Evita revelar que o email existe mas não é admin
            if (exigeAdmin && conta.Tipo != TipoUsuario.Admin)
                return Forbid();
 
            // Gera o token JWT com os dados da conta e do usuário
            var (token, expiraEm) = _jwtTokenService.GerarToken(conta);
 
            return Ok(new LoginResponseDTO
            {
                Token    = token,
                ExpiraEm = expiraEm,
                Nome     = conta.Usuario!.Nome,  // Nome pessoal vem do Usuario vinculado
                Tipo     = conta.Tipo.ToString()
            });
        }
    }
}