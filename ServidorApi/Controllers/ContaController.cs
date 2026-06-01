using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os endpoints exigem token válido — usuário ou admin
    public class ContaController : ControllerBase
    {
        private readonly IContaService _contaService;

        public ContaController(IContaService contaService)
        {
            _contaService = contaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContaResponseDTO dto)
        {
            var validator = new ContaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var novaConta = await _contaService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaConta.IDConta }, novaConta);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var conta = await _contaService.BuscarPorId(id);

            if (conta == null)
                return NotFound("Conta não encontrada.");

            return Ok(conta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contaService.Deletar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContaUpdateDTO dto)
        {
            await _contaService.Atualizar(id, dto);
            return NoContent();
        }
    }
}