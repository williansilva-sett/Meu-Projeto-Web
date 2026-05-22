using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Models;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]

    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService; // Seguindo seu padrão

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoriaResponseDTO dto) 
        {
            // 1. Validação (Pode usar o FluentValidation aqui também)
            var validator = new CategoriaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid) 
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            // 2. O Service cuida da lógica e o banco dispara o TRIGGER
            var novaCategoria = await _categoriaService.Criar(dto);
            
            return CreatedAtAction(nameof(GetById), new { id = novaCategoria.IDCategoria}, novaCategoria);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.BuscarPorId(id);
            
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada.");
            }

            return Ok(categoria);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            await _categoriaService.Deletar(id);
            
            return NoContent(); // Sucesso (204)
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            return Ok(await _categoriaService.ListarTodos());
        }

    }
}