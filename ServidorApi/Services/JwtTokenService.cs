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

        public (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
        {
            var expiraEm = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
            var role = usuario.Tipo.ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.ID.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, role)
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