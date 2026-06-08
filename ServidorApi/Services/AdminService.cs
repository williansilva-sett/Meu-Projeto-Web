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
 
        // ── PASSO 2.1 — Dashboard ─────────────────────────────────────────────
 
        public async Task<AdminDashboardDTO> ObterDashboardAsync()
        {
            // Conta usuários com tipo Usuario na tb_conta (não admins)
            var totalUsuarios = await _context.Contas
                .AsNoTracking()
                .CountAsync(c => c.Tipo == TipoUsuario.Usuario);
 
            // Total de contas de sistema criadas
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
                TotalUsuarios = totalUsuarios,
                TotalContas   = totalContas,
                TotalEntradas = totalEntradas,
                ValorEntradas = valorEntradas,
                TotalSaidas   = totalSaidas,
                ValorSaidas   = valorSaidas,
                SaldoGeral    = valorEntradas - valorSaidas
            };
        }
 
        // ── PASSO 2.2 — Usuários ──────────────────────────────────────────────
 
        public async Task<PaginadoDTO<AdminUsuarioListaDTO>> ListarUsuarios(
            string? busca, int page, int pageSize)
        {
            // Inclui a Conta para acessar Email, Tipo, Ativo e DataCriacao
            var query = _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Conta)
                .AsQueryable();
 
            // Busca por nome, sobrenome ou email (email está na Conta)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaLower = busca.ToLower().Trim();
                query = query.Where(u =>
                    u.Nome.ToLower().Contains(buscaLower) ||
                    u.Sobrenome.ToLower().Contains(buscaLower) ||
                    u.Conta!.Email.ToLower().Contains(buscaLower));
            }
 
            var totalItens = await query.CountAsync();
 
            // Ordena por data de criação da Conta (mais recentes primeiro)
            var usuarios = await query
                .OrderByDescending(u => u.Conta!.DataCriacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUsuarioListaDTO
                {
                    Id          = u.ID,
                    Nome        = u.Nome,
                    Sobrenome   = u.Sobrenome,
                    Email       = u.Conta!.Email,
                    Telefone    = u.Telefone,
                    Idade       = u.Idade,
                    Tipo        = u.Conta.Tipo.ToString(),
                    Ativo       = u.Conta.Ativo,
                    DataCriacao = u.Conta.DataCriacao
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
                .Include(u => u.Conta) // Carrega a conta de sistema
                .Where(u => u.ID == id)
                .Select(u => new AdminUsuarioDetalheDTO
                {
                    Id          = u.ID,
                    Nome        = u.Nome,
                    Sobrenome   = u.Sobrenome,
                    Email       = u.Conta!.Email,
                    Telefone    = u.Telefone,
                    Idade       = u.Idade,
                    Tipo        = u.Conta.Tipo.ToString(),
                    Ativo       = u.Conta.Ativo,
                    DataCriacao = u.Conta.DataCriacao,
                    Contas      = new List<AdminContaResumoDTO>() // Sem contas financeiras
                })
                .FirstOrDefaultAsync();
 
            return usuario;
        }
 
        public async Task<bool> AtualizarUsuario(int id, AdminUsuarioUpdateDTO dto)
        {
            // Atualiza dados pessoais no Usuario
            var usuario = await _context.Usuarios
                .Include(u => u.Conta)
                .FirstOrDefaultAsync(u => u.ID == id);
 
            if (usuario is null) return false;
 
            usuario.Telefone = dto.Telefone;
 
            // Email fica na Conta de sistema
            if (usuario.Conta is not null)
                usuario.Conta.Email = dto.Email.ToLower().Trim();
 
            await _context.SaveChangesAsync();
            return true;
        }
 
        public async Task<bool> AlterarStatusAtivo(int id, bool ativo)
        {
            // Ativo está na Conta de sistema, não no Usuario
            var usuario = await _context.Usuarios
                .Include(u => u.Conta)
                .FirstOrDefaultAsync(u => u.ID == id);
 
            if (usuario is null || usuario.Conta is null) return false;
 
            usuario.Conta.Ativo = ativo;
            await _context.SaveChangesAsync();
            return true;
        }
 
        public async Task<bool> ExcluirUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario is null) return false;
 
            // Cascade no DataContext garante que a Conta é deletada junto
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
 
        // ── PASSO 2.3 — Transações ────────────────────────────────────────────
 
        public async Task<PaginadoDTO<TransacaoAdminDTO>> ListarTransacoes(TransacaoFiltroDTO filtro)
        {
            if (filtro.Page < 1) filtro.Page = 1;
            if (filtro.PageSize < 1 || filtro.PageSize > 100) filtro.PageSize = 20;
 
            var tipo = filtro.Tipo.ToLower().Trim();
 
            // Entradas e Saídas ainda referenciam IDConta — será ajustado na Etapa 5
            var entradasQuery = _context.Entrada
                .AsNoTracking()
                .Include(e => e.Categoria)
                .AsQueryable();
 
            if (filtro.DataInicio.HasValue)
                entradasQuery = entradasQuery
                    .Where(e => e.Data >= filtro.DataInicio.Value);
 
            if (filtro.DataFim.HasValue)
                entradasQuery = entradasQuery
                    .Where(e => e.Data <= filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));
 
            var saidasQuery = _context.Saidas
                .AsNoTracking()
                .Include(s => s.Categoria)
                .AsQueryable();
 
            if (filtro.DataInicio.HasValue)
                saidasQuery = saidasQuery
                    .Where(s => s.DataSaida >= filtro.DataInicio.Value);
 
            if (filtro.DataFim.HasValue)
                saidasQuery = saidasQuery
                    .Where(s => s.DataSaida <= filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1));
 
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
                        IdConta       = 0,
                        NomeConta     = string.Empty,
                        IdCategoria   = e.IDCategoria,
                        NomeCategoria = e.Categoria!.categoria,
                        Descricao     = e.Descricao,
                        IdUsuario     = 0,
                        NomeUsuario   = string.Empty
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
                        IdConta       = 0,
                        NomeConta     = string.Empty,
                        IdCategoria   = s.IDCategoria,
                        NomeCategoria = s.Categoria!.categoria,
                        Descricao     = null,
                        IdUsuario     = 0,
                        NomeUsuario   = string.Empty
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