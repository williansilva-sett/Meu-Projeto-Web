using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;
 
namespace ServidorApi.Services
{
    public class MetaService : IMetaService
    {
        private readonly DataContext _context;
 
        public MetaService(DataContext context)
        {
            _context = context;
        }
 
        public async Task<IEnumerable<MetaResponseDTO>> ListarPorUsuario(int usuarioId)
        {
            // Retorna apenas as metas do usuário autenticado
            // Ordenadas por status (em andamento primeiro) e depois por data limite
            return await _context.Metas
                .AsNoTracking()
                .Where(m => m.IDUsuario == usuarioId)
                .OrderBy(m => m.Status)
                .ThenBy(m => m.DataLimite)
                .Select(m => new MetaResponseDTO
                {
                    ID         = m.ID,
                    Nome       = m.Nome,
                    Descricao  = m.Descricao,
                    ValorAlvo  = m.ValorAlvo,
                    ValorAtual = m.ValorAtual,
                    DataInicio = m.DataInicio,
                    DataLimite = m.DataLimite,
                    Status     = m.Status.ToString(),
                    IDUsuario  = m.IDUsuario
                })
                .ToListAsync();
        }
 
        public async Task<MetaResponseDTO?> BuscarPorId(int id)
        {
            return await _context.Metas
                .AsNoTracking()
                .Where(m => m.ID == id)
                .Select(m => new MetaResponseDTO
                {
                    ID         = m.ID,
                    Nome       = m.Nome,
                    Descricao  = m.Descricao,
                    ValorAlvo  = m.ValorAlvo,
                    ValorAtual = m.ValorAtual,
                    DataInicio = m.DataInicio,
                    DataLimite = m.DataLimite,
                    Status     = m.Status.ToString(),
                    IDUsuario  = m.IDUsuario
                })
                .FirstOrDefaultAsync();
        }
 
        public async Task<MetaResponseDTO> Criar(MetaCreateDTO dto)
        {
            // Verifica se o usuário existe antes de criar
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.ID == dto.IDUsuario);
 
            if (!usuarioExiste)
                throw new InvalidOperationException("Usuário não encontrado.");
 
            var meta = new Meta
            {
                Nome       = dto.Nome,
                Descricao  = dto.Descricao,
                ValorAlvo  = dto.ValorAlvo,
                ValorAtual = dto.ValorAtual,
                DataInicio = dto.DataInicio,
                DataLimite = dto.DataLimite,
                Status     = StatusMeta.EmAndamento, // Sempre começa em andamento
                IDUsuario  = dto.IDUsuario
            };
 
            _context.Metas.Add(meta);
            await _context.SaveChangesAsync();
 
            return new MetaResponseDTO
            {
                ID         = meta.ID,
                Nome       = meta.Nome,
                Descricao  = meta.Descricao,
                ValorAlvo  = meta.ValorAlvo,
                ValorAtual = meta.ValorAtual,
                DataInicio = meta.DataInicio,
                DataLimite = meta.DataLimite,
                Status     = meta.Status.ToString(),
                IDUsuario  = meta.IDUsuario
            };
        }
 
        public async Task<bool> Atualizar(int id, MetaUpdateDTO dto)
        {
            var meta = await _context.Metas.FindAsync(id);
            if (meta is null) return false;
 
            meta.Nome       = dto.Nome;
            meta.Descricao  = dto.Descricao;
            meta.ValorAlvo  = dto.ValorAlvo;
            meta.DataLimite = dto.DataLimite;
 
            await _context.SaveChangesAsync();
            return true;
        }
 
        public async Task<bool> AtualizarProgresso(int id, MetaProgressoDTO dto)
        {
            var meta = await _context.Metas.FindAsync(id);
            if (meta is null) return false;
 
            meta.ValorAtual = dto.ValorAtual;
 
            // Conclui automaticamente se o valor atual atingiu o alvo
            if (meta.ValorAtual >= meta.ValorAlvo)
                meta.Status = StatusMeta.Concluida;
 
            await _context.SaveChangesAsync();
            return true;
        }
 
        public async Task<bool> AtualizarStatus(int id, MetaStatusDTO dto)
        {
            var meta = await _context.Metas.FindAsync(id);
            if (meta is null) return false;
 
            // Converte a string para o enum — retorna false se inválido
            if (!Enum.TryParse<StatusMeta>(dto.Status, out var status))
                return false;
 
            meta.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
 
        public async Task<bool> Deletar(int id)
        {
            var meta = await _context.Metas.FindAsync(id);
            if (meta is null) return false;
 
            _context.Metas.Remove(meta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}