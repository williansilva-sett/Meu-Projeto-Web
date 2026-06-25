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
    public class EntradasController : ControllerBase
    {
        private readonly IEntradaService _entradaService;

        public EntradasController(IEntradaService entradaService)
        {
            _entradaService = entradaService;
        }

        private int ObterUsuarioId()
        {
            var claim = User.FindFirstValue("usuarioId");
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private bool EhAdmin() => User.IsInRole("Admin");

        [HttpPost]
        public async Task<IActionResult> Create(EntradasResponseDTO dto)
        {
            // Força o dono pelo token - antes o cliente podia mandar
            // qualquer IDUsuario e a API aceitava sem checar.
            dto.IDUsuario = ObterUsuarioId();

            var validator = new EntradasValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var novaEntrada = await _entradaService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaEntrada.IDEntrada }, novaEntrada);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entrada = await _entradaService.BuscarPorId(id);

            if (entrada == null)
                return NotFound("Entrada não encontrada.");

            if (entrada.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            return Ok(entrada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entrada = await _entradaService.BuscarPorId(id);

            if (entrada == null)
                return NotFound("Entrada não encontrada.");

            if (entrada.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            await _entradaService.Deletar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntradasUpdateDTO dto)
        {
            var entrada = await _entradaService.BuscarPorId(id);

            if (entrada == null)
                return NotFound("Entrada não encontrada.");

            if (entrada.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            await _entradaService.Atualizar(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todas = await _entradaService.ListarTodos();

            if (EhAdmin())
                return Ok(todas);

            var usuarioId = ObterUsuarioId();
            return Ok(todas.Where(e => e.IDUsuario == usuarioId));
        }
    }
}