using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    [Authorize]
    public class LeccionesController : Controller
    {
        private readonly ILeccionService _leccionService;
        private readonly ICursoService _cursoService;
        private readonly IProgresoService _progresoService;
        private readonly IInscripcionService _inscripcionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LeccionesController> _logger;

        public LeccionesController(
            ILeccionService leccionService,
            ICursoService cursoService,
            IProgresoService progresoService,
            IInscripcionService inscripcionService,
            UserManager<ApplicationUser> userManager,
            ILogger<LeccionesController> logger)
        {
            _leccionService = leccionService;
            _cursoService = cursoService;
            _progresoService = progresoService;
            _inscripcionService = inscripcionService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Lecciones/Ver?cursoId=1&leccionId=5
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> Ver(int cursoId, int leccionId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            // Verificar acceso (inscripción activa), admin siempre puede
            if (!User.IsInRole(RolesConstantes.Administrador))
            {
                bool tieneAcceso = await _progresoService.PuedeAccederLeccionAsync(userId, leccionId);
                if (!tieneAcceso)
                {
                    TempData["Error"] = "Debes inscribirte en el curso para acceder a las lecciones.";
                    return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
                }
            }

            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(cursoId);
            if (curso == null) return NotFound();

            var leccion = await _leccionService.ObtenerLeccionDeCursoAsync(cursoId, leccionId);
            if (leccion == null) return NotFound();

            // Lecciones del curso para el panel lateral
            var todasLasLecciones = await _leccionService.ObtenerLeccionesPorCursoAsync(cursoId);

            // Lecciones completadas por el usuario
            var leccionesCompletadas = await _progresoService.ObtenerLeccionesCompletadasAsync(userId, cursoId);

            // Navegación siguiente / anterior
            var siguiente = await _leccionService.ObtenerSiguienteLeccionAsync(cursoId, leccion.Orden);
            var anterior = await _leccionService.ObtenerLeccionAnteriorAsync(cursoId, leccion.Orden);

            // Progreso general
            double progreso = await _progresoService.ObtenerProgresoAsync(userId, cursoId);
            bool estaCompletada = await _progresoService.EstaLeccionCompletadaAsync(userId, leccionId);

            ViewData["Curso"] = curso;
            ViewData["TodasLasLecciones"] = todasLasLecciones;
            ViewData["LeccionesCompletadas"] = leccionesCompletadas;
            ViewData["LeccionSiguiente"] = siguiente;
            ViewData["LeccionAnterior"] = anterior;
            ViewData["Progreso"] = progreso;
            ViewData["EstaCompletada"] = estaCompletada;

            return View(leccion);
        }

        // POST: /Lecciones/MarcarCompletada
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> MarcarCompletada(int leccionId, int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            try
            {
                await _progresoService.MarcarLeccionCompletadaAsync(userId, leccionId);
                TempData["Success"] = "¡Lección marcada como completada!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar lección {LeccionId} como completada para {UserId}", leccionId, userId);
                TempData["Error"] = "No se pudo registrar el progreso. Intenta de nuevo.";
            }

            return RedirectToAction("Ver", new { cursoId, leccionId });
        }
    }
}
