using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;
using System.Security.Claims;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaidaController : ControllerBase
    {
        private readonly ISaidaService _saidaService;

        public SaidaController(ISaidaService saidaService)
        {
            _saidaService = saidaService;
        }

        private int ObterUsuarioId()
        {
            var claim = User.FindFirstValue("usuarioId");
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private bool EhAdmin() => User.IsInRole("Admin");

        [HttpPost]
        public async Task<IActionResult> Create(SaidaResponseDTO dto)
        {
            dto.IDUsuario = ObterUsuarioId();

            var validator = new SaidaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var novaSaida = await _saidaService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaSaida.IDSaida }, novaSaida);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var saida = await _saidaService.BuscarPorId(id);

            if (saida == null)
                return NotFound("Saída não encontrada.");

            if (saida.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            return Ok(saida);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var saida = await _saidaService.BuscarPorId(id);

            if (saida == null)
                return NotFound("Saída não encontrada.");

            if (saida.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            await _saidaService.Deletar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaidaUpdateDTO dto)
        {
            var saida = await _saidaService.BuscarPorId(id);

            if (saida == null)
                return NotFound("Saída não encontrada.");

            if (saida.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            await _saidaService.Atualizar(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todas = await _saidaService.ListarTodos();

            if (EhAdmin())
                return Ok(todas);

            var usuarioId = ObterUsuarioId();
            return Ok(todas.Where(s => s.IDUsuario == usuarioId));
        }
    }
}