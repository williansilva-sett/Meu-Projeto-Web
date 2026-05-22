using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;


namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly IContaService _contaService; // Seguindo seu padrão

        public ContaController(IContaService contaService)
        {
            _contaService = contaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContaResponseDTO dto) 
        {
            // 1. Validação (Pode usar o FluentValidation aqui também)
            var validator = new ContaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            // 2. O Service cuida da lógica e o banco dispara o TRIGGER
            var novaConta = await _contaService.Criar(dto);
            
            return CreatedAtAction(nameof(GetById), new { id = novaConta.IDConta }, novaConta);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var conta = await _contaService.BuscarPorId(id);
            
            if (conta == null)
            {
                return NotFound("Conta não encontrada.");
            }

            return Ok(conta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            await _contaService.Deletar(id);
            
            return NoContent(); // Sucesso (204)
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContaUpdateDTO dto)
        {
            await _contaService.Atualizar(id, dto);
            return NoContent();
        }
    }
}