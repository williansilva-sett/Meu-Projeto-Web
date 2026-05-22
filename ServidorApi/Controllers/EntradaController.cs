using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;


namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntradasController : ControllerBase
    {
        private readonly IEntradaService _entradaService; // Seguindo seu padrão

        public EntradasController(IEntradaService entradaService)
        {
            _entradaService = entradaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(EntradasResponseDTO dto) 
        {
            // 1. Validação (Pode usar o FluentValidation aqui também)
            var validator = new EntradasValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            // 2. O Service cuida da lógica e o banco dispara o TRIGGER
            var novaEntrada = await _entradaService.Criar(dto);
            
            return CreatedAtAction(nameof(GetById), new { id = novaEntrada.IDEntrada }, novaEntrada);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entrada = await _entradaService.BuscarPorId(id);
            
            if (entrada == null)
            {
                return NotFound("Entrada não encontrada.");
            }

            return Ok(entrada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            await _entradaService.Deletar(id);
            
            return NoContent(); // Sucesso (204)
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