// ServidorApi/Services/AdminService.cs
// SUBSTITUIR o arquivo existente por este completo
// Remove toda referência a "Ativo" que não existe no Usuario ainda

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

        // ── PASSO 2.1 ─────────────────────────────────────────────────────────

        public async Task<AdminDashboardDTO> ObterDashboardAsync()
        {
            // Conta apenas usuários com tipo Usuario (não conta admins)
            var totalUsuarios = await _context.Usuarios
                .AsNoTracking()
                .CountAsync(u => u.Tipo == TipoUsuario.Usuario);

            var totalContas = await _context.Contas
                .AsNoTracking()
                .CountAsync();

            var totalEntradas = await _context.Entrada
                .AsNoTracking()
                .CountAsync();

            var valorEntradas = await _context.Entrada
                .AsNoTracking()
                .SumAsync(e => (decimal?)e.ValorEntrada) ?? 0;

            var totalSaidas = await _context.Saidas
                .AsNoTracking()
                .CountAsync();

            var valorSaidas = await _context.Saidas
                .AsNoTracking()
                .SumAsync(s => (decimal?)s.ValorSaida) ?? 0;

            return new AdminDashboardDTO
            {
                TotalUsuarios  = totalUsuarios,
                TotalContas    = totalContas,
                TotalEntradas  = totalEntradas,
                ValorEntradas  = valorEntradas,
                TotalSaidas    = totalSaidas,
                ValorSaidas    = valorSaidas,
                SaldoGeral     = valorEntradas - valorSaidas
            };
        }

        // ── PASSO 2.2 ─────────────────────────────────────────────────────────

        public async Task<PaginadoDTO<AdminUsuarioListaDTO>> ListarUsuarios(
            string? busca, int page, int pageSize)
        {
            var query = _context.Usuarios.AsNoTracking().AsQueryable();

            // Aplica filtro de busca por nome, sobrenome ou email
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaLower = busca.ToLower().Trim();
                query = query.Where(u =>
                    u.Nome.ToLower().Contains(buscaLower) ||
                    u.Sobrenome.ToLower().Contains(buscaLower) ||
                    u.Email.ToLower().Contains(buscaLower));
                    
            }

            // Conta o total antes de paginar para o frontend calcular páginas
            var totalItens = await query.CountAsync();

            var usuarios = await query
                .OrderByDescending(u => u.DataCriacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUsuarioListaDTO
                {
                    Id          = u.ID,
                    Nome        = u.Nome,
                    Sobrenome   = u.Sobrenome,
                    Email       = u.Email,
                    Telefone    = u.Telefone,
                    Idade       = u.Idade,
                    Tipo        = u.Tipo.ToString(),
                    Ativo       = u.Ativo,
                    DataCriacao = u.DataCriacao
                    
                })
                .ToListAsync();

            return new PaginadoDTO<AdminUsuarioListaDTO>
            {
                Itens         = usuarios,
                TotalItens    = totalItens,
                PaginaAtual   = page,
                TamanhoPagina = pageSize
            };
        }

        public async Task<AdminUsuarioDetalheDTO?> ObterUsuarioPorId(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Contas)
                .Where(u => u.ID == id)
                .Select(u => new AdminUsuarioDetalheDTO
                {
                    Id          = u.ID,
                    Nome        = u.Nome,
                    Sobrenome   = u.Sobrenome,
                    Email       = u.Email,
                    Telefone    = u.Telefone,
                    Idade       = u.Idade,
                    Tipo        = u.Tipo.ToString(),
                    Ativo       = u.Ativo,
                    DataCriacao = u.DataCriacao,
                    Contas      = u.Contas.Select(c => new AdminContaResumoDTO
                    {
                        IDConta   = c.IDConta,
                        NomeConta = c.NomeConta,
                        Ativa     = c.Ativa
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return usuario;
        }

        public async Task<bool> AtualizarUsuario(int id, AdminUsuarioUpdateDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario is null) return false;

            usuario.Telefone = dto.Telefone;
            usuario.Email    = dto.Email.ToLower().Trim();

            await _context.SaveChangesAsync();
            return true;
        }

        // Ativo não existe ainda no model — método mantido para quando o campo for adicionado
        public async Task<bool> AlterarStatusAtivo(int id, bool ativo)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario is null) return false;
            usuario.Ativo = ativo;
            await _context.SaveChangesAsync();

            return await Task.FromResult(false);
        }

        public async Task<bool> ExcluirUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario is null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── PASSO 2.3 ─────────────────────────────────────────────────────────

        public async Task<PaginadoDTO<TransacaoAdminDTO>> ListarTransacoes(TransacaoFiltroDTO filtro)
        {
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1 || filtro.PageSize > 100) filtro.PageSize = 20;

            var tipo = filtro.Tipo.ToLower().Trim();

            var entradasQuery = _context.Entrada
                .AsNoTracking()
                .Include(e => e.Conta).ThenInclude(c => c!.Usuario)
                .Include(e => e.Categoria)
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                entradasQuery = entradasQuery.Where(e => e.Data >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                entradasQuery = entradasQuery
                    .Where(e => e.Data <= filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));

            if (filtro.IdConta.HasValue)
                entradasQuery = entradasQuery.Where(e => e.IDConta == filtro.IdConta.Value);

            var saidasQuery = _context.Saidas
                .AsNoTracking()
                .Include(s => s.Conta).ThenInclude(c => c!.Usuario)
                .Include(s => s.Categoria)
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                saidasQuery = saidasQuery.Where(s => s.DataSaida >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                saidasQuery = saidasQuery
                    .Where(s => s.DataSaida <= filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));

            if (filtro.IdConta.HasValue)
                saidasQuery = saidasQuery.Where(s => s.IDConta == filtro.IdConta.Value);

            List<TransacaoAdminDTO> transacoes = new();

            if (tipo == "entrada" || tipo == "todos")
            {
                var entradas = await entradasQuery
                    .Select(e => new TransacaoAdminDTO
                    {
                        Id            = e.IDEntrada,
                        Tipo          = "Entrada",
                        Valor         = e.ValorEntrada,
                        Data          = e.Data,
                        IdConta       = e.IDConta,
                        NomeConta     = e.Conta!.NomeConta,
                        IdCategoria   = e.IDCategoria,
                        NomeCategoria = e.Categoria!.categoria,
                        Descricao     = e.Descricao,
                        IdUsuario     = e.Conta.UsuarioID,
                        NomeUsuario   = e.Conta.Usuario!.Nome + " " + e.Conta.Usuario.Sobrenome
                    })
                    .ToListAsync();

                transacoes.AddRange(entradas);
            }

            if (tipo == "saida" || tipo == "todos")
            {
                var saidas = await saidasQuery
                    .Select(s => new TransacaoAdminDTO
                    {
                        Id            = s.IDSaida,
                        Tipo          = "Saida",
                        Valor         = s.ValorSaida,
                        Data          = s.DataSaida,
                        IdConta       = s.IDConta,
                        NomeConta     = s.Conta!.NomeConta,
                        IdCategoria   = s.IDCategoria,
                        NomeCategoria = s.Categoria!.categoria,
                        Descricao     = null,
                        IdUsuario     = s.Conta.UsuarioID,
                        NomeUsuario   = s.Conta.Usuario!.Nome + " " + s.Conta.Usuario.Sobrenome
                    })
                    .ToListAsync();

                transacoes.AddRange(saidas);
            }

            var totalItens = transacoes.Count;

            var itensPaginados = transacoes
                .OrderByDescending(t => t.Data)
                .Skip((filtro.Page - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToList();

            return new PaginadoDTO<TransacaoAdminDTO>
            {
                Itens         = itensPaginados,
                TotalItens    = totalItens,
                PaginaAtual   = filtro.Page,
                TamanhoPagina = filtro.PageSize
            };
        }
    }
}