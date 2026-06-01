using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os endpoints exigem token válido — usuário ou admin
    public class SaidaController : ControllerBase
    {
        private readonly ISaidaService _saidaService;

        public SaidaController(ISaidaService saidaService)
        {
            _saidaService = saidaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaidaResponseDTO dto)
        {
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

            return Ok(saida);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _saidaService.Deletar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaidaUpdateDTO dto)
        {
            await _saidaService.Atualizar(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _saidaService.ListarTodos());
        }
    }
}