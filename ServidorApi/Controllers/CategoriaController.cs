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
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        private int ObterUsuarioId()
        {
            var claim = User.FindFirstValue("usuarioId");
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private bool EhAdmin() => User.IsInRole("Admin");

        // POST /api/Categoria - qualquer usuário autenticado pode criar
        [HttpPost]
        public async Task<IActionResult> Create(CategoriaResponseDTO dto)
        {
            // Admin pode criar categoria global (IDUsuario = null se não
            // informar); usuário comum sempre cria a SUA própria - aqui
            // a gente ignora o que vier no body e força pelo token,
            // pra ninguém criar categoria em nome de outro usuário.
            dto.IDUsuario = EhAdmin() ? dto.IDUsuario : ObterUsuarioId();

            var validator = new CategoriaValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var novaCategoria = await _categoriaService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaCategoria.IDCategoria }, novaCategoria);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _categoriaService.BuscarPorId(id);

            if (categoria == null)
                return NotFound("Categoria não encontrada.");

            // Global (IDUsuario null) - todo mundo vê.
            // Pessoal - só o dono ou Admin.
            if (categoria.IDUsuario != null && categoria.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            return Ok(categoria);
        }

        // DELETE - agora liberado pro dono também, não só Admin
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaService.BuscarPorId(id);

            if (categoria == null)
                return NotFound("Categoria não encontrada.");

            if (categoria.IDUsuario != ObterUsuarioId() && !EhAdmin())
                return Forbid();

            await _categoriaService.Deletar(id);
            return NoContent();
        }

        // GET - globais + as do próprio usuário; Admin vê tudo sem filtro
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todas = await _categoriaService.ListarTodos();

            if (EhAdmin())
                return Ok(todas);

            var usuarioId = ObterUsuarioId();
            return Ok(todas.Where(c => c.IDUsuario == null || c.IDUsuario == usuarioId));
        }
    }
}