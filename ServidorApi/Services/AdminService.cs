using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;

        public AdminService(DataContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDTO> ObterDashboardAsync()
        {
            // EF Core NÃO suporta queries paralelas no mesmo DbContext
            // → executar sequencialmente com await individual

            var totalUsuarios = await _context.Usuarios
                .AsNoTracking()
                .CountAsync(u => u.Tipo == TipoUsuario.Usuario);

            var totalContas = await _context.Contas
                .AsNoTracking()
                .CountAsync();

            var entradasAgregado = await _context.Entrada
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Valor = g.Sum(e => e.ValorEntrada)
                })
                .FirstOrDefaultAsync();

            var saidasAgregado = await _context.Saidas
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Valor = g.Sum(s => s.ValorSaida)
                })
                .FirstOrDefaultAsync();

            var valorEntradas = entradasAgregado?.Valor ?? 0m;
            var valorSaidas   = saidasAgregado?.Valor   ?? 0m;

            return new AdminDashboardDTO
            {
                TotalUsuarios = totalUsuarios,
                TotalContas   = totalContas,
                TotalEntradas = entradasAgregado?.Total ?? 0,
                ValorEntradas = valorEntradas,
                TotalSaidas   = saidasAgregado?.Total   ?? 0,
                ValorSaidas   = valorSaidas,
                SaldoGeral    = valorEntradas - valorSaidas,
                DataConsulta  = DateTime.UtcNow
            };
        }
    }
}