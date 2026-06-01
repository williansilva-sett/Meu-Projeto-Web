// ServidorApi/Controllers/AdminController.cs
// SUBSTITUIR o arquivo existente por este

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── PASSO 2.1 ─────────────────────────────────────────────────────────

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _adminService.ObterDashboardAsync();
            return Ok(dashboard);
        }

        // ── PASSO 2.2 ─────────────────────────────────────────────────────────

        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios(
            [FromQuery] string? busca,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var resultado = await _adminService.ListarUsuarios(busca, page, pageSize);
            return Ok(resultado);
        }

        [HttpGet("usuarios/{id}")]
        public async Task<IActionResult> ObterUsuario(int id)
        {
            var usuario = await _adminService.ObterUsuarioPorId(id);

            if (usuario is null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return Ok(usuario);
        }

        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> AtualizarUsuario(
            int id, [FromBody] AdminUsuarioUpdateDTO dto)
        {
            var sucesso = await _adminService.AtualizarUsuario(id, dto);

            if (!sucesso)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return NoContent();
        }

        [HttpPatch("usuarios/{id}/ativo")]
        public async Task<IActionResult> AlterarStatusAtivo(int id, [FromBody] bool ativo)
        {
            var sucesso = await _adminService.AlterarStatusAtivo(id, ativo);
            if (!sucesso) return NotFound(new { mensagem = "Usuário não encontrado." });
            return NoContent();
        }

        [HttpDelete("usuarios/{id}")]
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            var sucesso = await _adminService.ExcluirUsuario(id);

            if (!sucesso)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return NoContent();
        }

        // ── PASSO 2.3 ─────────────────────────────────────────────────────────

        [HttpGet("transacoes")]
        public async Task<IActionResult> ListarTransacoes([FromQuery] TransacaoFiltroDTO filtro)
        {
            var tiposValidos = new[] { "entrada", "saida", "todos" };
            if (!tiposValidos.Contains(filtro.Tipo.ToLower()))
                return BadRequest(new { mensagem = "Tipo inválido. Use: entrada, saida ou todos." });

            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue
                && filtro.DataFim < filtro.DataInicio)
                return BadRequest(new { mensagem = "dataFim não pode ser anterior a dataInicio." });

            var resultado = await _adminService.ListarTransacoes(filtro);
            return Ok(resultado);
        }
    }
}