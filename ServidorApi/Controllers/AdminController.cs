using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServidorApi.Data;
using ServidorApi.Models;

namespace ServidorApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]   // ← protege todo o controller
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminController(DataContext context)
        {
            _context = context;
        }

        // GET /api/admin/dashboard
        // ✅ 200 com token Admin | 401 sem token | 403 role errada
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalAdmins   = await _context.Usuarios
                                    .CountAsync(u => u.Tipo == TipoUsuario.Admin);

            return Ok(new
            {
                totalUsuarios,
                totalAdmins,
                dataConsulta = DateTime.UtcNow
            });
        }
    }
}