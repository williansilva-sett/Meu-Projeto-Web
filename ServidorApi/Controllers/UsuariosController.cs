using Microsoft.AspNetCore.Mvc;
using ServidorApi.DTOs;
using ServidorApi.Services.Interfaces;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // O Controller agora só conhece o Service. 
        // A "sujeira" de banco de dados fica no Service.
        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            return Ok(await _usuarioService.ListarTodos());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) 
        {
            var usuario = await _usuarioService.BuscarPorId(id);
            if (usuario == null) return NotFound("Usuário não encontrado.");
            return Ok(usuario);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            await _usuarioService.Deletar(id);
            return NoContent();
        }

        [HttpGet("filtrar")]
        public IActionResult GetFiltrado([FromQuery] UsuarioResponseDTO request)
        {
            // O .NET vai usar o validador automaticamente se estiver registrado no Program.cs
            return Ok($"Buscando usuários com {request.Idade} anos.");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UsuarioCreateDTO dto) 
        {
            // 1. Validamos manualmente aqui
            var validator = new UsuarioCreateValidator();
            var validationResult = await validator.ValidateAsync(dto);

            // 2. Se houver erro (ex: idade < 18), ele para aqui e avisa o usuário
            if (!validationResult.IsValid) 
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // 3. Só chega aqui se for maior de idade
            var novoUsuario = await _usuarioService.Criar(dto);
            return CreatedAtAction(nameof(GetById), new { id = novoUsuario.Id }, novoUsuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpDateDTO dto)
        {
            await _usuarioService.Atualizar(id, dto);
            return NoContent();
        }
    }
}