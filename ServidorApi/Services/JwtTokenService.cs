using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServidorApi.Configuration;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
 
namespace ServidorApi.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
 
        public JwtTokenService(IOptions<JwtSettings> jwtOptions) =>
            _jwtSettings = jwtOptions.Value;
 
        public (string Token, DateTime ExpiraEm) GerarToken(Conta conta)
        {
            var expiraEm = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
 
            var claims = new[]
            {
                // ID da conta de sistema — usado para identificar o usuário nas rotas protegidas
                new Claim(ClaimTypes.NameIdentifier, conta.ID.ToString()),
 
                // Email da conta — identificador de acesso
                new Claim(ClaimTypes.Email, conta.Email),
 
                // Nome pessoal do usuário — vem via navegação conta.Usuario
                // Usado para exibir no frontend sem precisar de outra requisição
                new Claim(ClaimTypes.Name, conta.Usuario!.Nome),
 
                // Role — "Usuario" ou "Admin" — usado pelo [Authorize(Roles = "Admin")]
                new Claim(ClaimTypes.Role, conta.Tipo.ToString()),
 
                // ID do usuário pessoal — útil para buscar dados pessoais nas rotas
                new Claim("usuarioId", conta.UsuarioID.ToString())
            };
 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiraEm,
                signingCredentials: credentials);
 
            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
        }
    }
}