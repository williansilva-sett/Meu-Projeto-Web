using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;
using System.Security.Claims;
 
namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todas as rotas exigem token JWT válido
    public class MetaController : ControllerBase
    {
        private readonly IMetaService _metaService;
 
        public MetaController(IMetaService metaService)
        {
            _metaService = metaService;
        }
 
        // Extrai o IDUsuario do token JWT
        // O token guarda o claim "usuarioId" com o ID do Usuario (não da Conta)
        private int ObterUsuarioId()
        {
            var claim = User.FindFirstValue("usuarioId");
            return int.TryParse(claim, out var id) ? id : 0;
        }
 
        // GET /api/meta
        // Lista todas as metas do usuário autenticado
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0) return Unauthorized();
 
            var metas = await _metaService.ListarPorUsuario(usuarioId);
            return Ok(metas);
        }
 
        // GET /api/meta/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var meta = await _metaService.BuscarPorId(id);
 
            if (meta is null)
                return NotFound(new { mensagem = "Meta não encontrada." });
 
            // Garante que o usuário só acessa suas próprias metas
            var usuarioId = ObterUsuarioId();
            if (meta.IDUsuario != usuarioId)
                return Forbid();
 
            return Ok(meta);
        }
 
        // POST /api/meta
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MetaCreateDTO dto)
        {
            // Força o IDUsuario do token — evita criar meta para outro usuário
            dto.IDUsuario = ObterUsuarioId();
            if (dto.IDUsuario == 0) return Unauthorized();
 
            try
            {
                var novaMeta = await _metaService.Criar(dto);
                return CreatedAtAction(nameof(GetById), new { id = novaMeta.ID }, novaMeta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
 
        // PUT /api/meta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MetaUpdateDTO dto)
        {
            var sucesso = await _metaService.Atualizar(id, dto);
 
            if (!sucesso)
                return NotFound(new { mensagem = "Meta não encontrada." });
 
            return NoContent();
        }
 
        // PATCH /api/meta/5/progresso
        // Atualiza o valor atual da meta
        [HttpPatch("{id}/progresso")]
        public async Task<IActionResult> AtualizarProgresso(int id, [FromBody] MetaProgressoDTO dto)
        {
            var sucesso = await _metaService.AtualizarProgresso(id, dto);
 
            if (!sucesso)
                return NotFound(new { mensagem = "Meta não encontrada." });
 
            return NoContent();
        }
 
        // PATCH /api/meta/5/status
        // Atualiza o status da meta (EmAndamento, Concluida, Cancelada)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] MetaStatusDTO dto)
        {
            var sucesso = await _metaService.AtualizarStatus(id, dto);
 
            if (!sucesso)
                return BadRequest(new { mensagem = "Meta não encontrada ou status inválido." });
 
            return NoContent();
        }
 
        // DELETE /api/meta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await _metaService.Deletar(id);
 
            if (!sucesso)
                return NotFound(new { mensagem = "Meta não encontrada." });
 
            return NoContent();
        }
    }
}