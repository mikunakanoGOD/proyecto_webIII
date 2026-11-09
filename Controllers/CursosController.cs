using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    public class CursosController : Controller
    {
        private readonly ICursoService _cursoService;
        private readonly IInscripcionService _inscripcionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CursosController> _logger;

        public CursosController(
            ICursoService cursoService,
            IInscripcionService inscripcionService,
            UserManager<ApplicationUser> userManager,
            ILogger<CursosController> logger)
        {
            _cursoService = cursoService;
            _inscripcionService = inscripcionService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Cursos
        public async Task<IActionResult> Index(string? nivel, string? busqueda)
        {
            IEnumerable<Curso> cursos;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                cursos = await _cursoService.BuscarCursosPorTituloAsync(busqueda);
                ViewData["Busqueda"] = busqueda;
            }
            else if (!string.IsNullOrWhiteSpace(nivel))
            {
                cursos = await _cursoService.ObtenerCursosPorNivelAsync(nivel);
                ViewData["NivelSeleccionado"] = nivel;
            }
            else
            {
                cursos = await _cursoService.ObtenerCursosPublicadosAsync();
            }

            return View(cursos);
        }

        // GET: /Cursos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(id);

            if (curso == null)
                return NotFound();

            if (!curso.EstaPublicado && !User.IsInRole(RolesConstantes.Administrador))
                return NotFound();

            // Información de inscripción para usuarios autenticados
            bool estaInscrito = false;
            int totalInscritos = 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != null)
                    estaInscrito = await _inscripcionService.VerificarAccesoAsync(userId, id);
            }

            totalInscritos = await _inscripcionService.ContarInscriptosAsync(id);

            ViewData["EstaInscrito"] = estaInscrito;
            ViewData["TotalInscritos"] = totalInscritos;

            return View(curso);
        }
    }
}
