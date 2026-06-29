using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ISenhaHasher _senhaHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(
            DataContext context,
            ISenhaHasher senhaHasher,
            IJwtTokenService jwtTokenService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _senhaHasher = senhaHasher;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
            _configuration = configuration;
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

            var conta = await _context.Contas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Email == dto.Email.ToLower().Trim());

            if (conta is null)
            {
                _senhaHasher.Verify("dummy", BCrypt.Net.BCrypt.HashPassword("dummy"));
                return Unauthorized("Email ou senha inválidos.");
            }

            if (conta.BloqueadoAte.HasValue && conta.BloqueadoAte > DateTime.UtcNow)
            {
                var minutosRestantes = (int)(conta.BloqueadoAte.Value - DateTime.UtcNow).TotalMinutes + 1;
                return StatusCode(429, $"Conta bloqueada. Tente novamente em {minutosRestantes} minuto(s).");
            }

            if (!_senhaHasher.Verify(dto.Senha, conta.Senha))
            {
                conta.TentativasLogin++;

                if (conta.TentativasLogin >= 5)
                {
                    conta.BloqueadoAte    = DateTime.UtcNow.AddMinutes(15);
                    conta.TentativasLogin = 0;
                }

                await _context.SaveChangesAsync();
                return Unauthorized("Email ou senha inválidos.");
            }

            conta.TentativasLogin = 0;
            conta.BloqueadoAte    = null;
            await _context.SaveChangesAsync();

            if (exigeAdmin && conta.Tipo != TipoUsuario.Admin)
                return Forbid();

            var (token, expiraEm) = _jwtTokenService.GerarToken(conta);

            return Ok(new LoginResponseDTO
            {
                Token    = token,
                ExpiraEm = expiraEm,
                Nome     = conta.Usuario!.Nome,
                Tipo     = conta.Tipo.ToString()
            });
        }

        // POST /api/auth/recuperar-senha
        // Gera um token de recuperação e envia por email - se o email
        // existir. Não revela se o email existe ou não na resposta, pra
        // evitar que alguém descubra quais emails estão cadastrados.
        [HttpPost("recuperar-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email é obrigatório.");

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Email == dto.Email.ToLower().Trim());

            if (conta is not null)
            {
                var token = GerarTokenSeguro();

                _context.TokensRecuperacaoSenha.Add(new TokenRecuperacaoSenha
                {
                    Token         = token,
                    ContaID       = conta.ID,
                    DataExpiracao = DateTime.UtcNow.AddMinutes(30),
                });

                await _context.SaveChangesAsync();

                var frontendUrl = _configuration["FrontendBaseUrl"] ?? "http://127.0.0.1:5500";
                var link = $"{frontendUrl}/Inicio/HTML/redefinir-senha.html?token={token}";

                await _emailService.EnviarAsync(
                    conta.Email,
                    "Recuperação de senha - Viva Finanças",
                    $"""
                    <p>Você solicitou a redefinição da sua senha no Viva Finanças.</p>
                    <p><a href="{link}">Clique aqui para criar uma nova senha</a></p>
                    <p>Esse link expira em 30 minutos. Se você não solicitou isso, pode ignorar este email.</p>
                    """);
            }

            // Mesma resposta, exista ou não a conta.
            return Ok(new { mensagem = "Se o email existir em nossa base, você receberá as instruções." });
        }

        // POST /api/auth/redefinir-senha
        [HttpPost("redefinir-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NovaSenha))
                return BadRequest("Token e nova senha são obrigatórios.");

            var tokenRecuperacao = await _context.TokensRecuperacaoSenha
                .Include(t => t.Conta)
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (tokenRecuperacao is null
                || tokenRecuperacao.Usado
                || tokenRecuperacao.DataExpiracao < DateTime.UtcNow)
            {
                return BadRequest("Token inválido ou expirado.");
            }

            tokenRecuperacao.Conta!.Senha = _senhaHasher.Hash(dto.NovaSenha);
            tokenRecuperacao.Usado = true;

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Senha redefinida com sucesso." });
        }

        private static string GerarTokenSeguro()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}