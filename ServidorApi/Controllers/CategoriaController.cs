using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize] // Todos os endpoints exigem token válido — usuário ou admin
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // Criar categoria é restrito ao Admin
        // Um usuário comum não deve poder criar categorias globais do sistema
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoriaResponseDTO dto)
        {
            var validator = new CategoriaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var novaCategoria = await _categoriaService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaCategoria.IDCategoria }, novaCategoria);
        }

        // Buscar e listar categorias: qualquer usuário autenticado pode ver
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.BuscarPorId(id);

            if (categoria == null)
                return NotFound("Categoria não encontrada.");

            return Ok(categoria);
        }

        // Deletar categoria restrito ao Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoriaService.Deletar(id);
            return NoContent();
        }

        // Listar categorias: qualquer usuário autenticado pode ver
        // Necessário para popular dropdowns nas telas de entrada/saída
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoriaService.ListarTodos());
        }
    }
}