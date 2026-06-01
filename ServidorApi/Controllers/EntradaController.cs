using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os endpoints exigem token válido — usuário ou admin
    public class EntradasController : ControllerBase
    {
        private readonly IEntradaService _entradaService;

        public EntradasController(IEntradaService entradaService)
        {
            _entradaService = entradaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(EntradasResponseDTO dto)
        {
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

            return Ok(entrada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _entradaService.Deletar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntradasUpdateDTO dto)
        {
            await _entradaService.Atualizar(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _entradaService.ListarTodos());
        }
    }
}