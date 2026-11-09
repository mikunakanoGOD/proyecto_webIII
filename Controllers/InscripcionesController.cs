using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    [Authorize]
    public class InscripcionesController : Controller
    {
        private readonly IInscripcionService _inscripcionService;
        private readonly ICursoService _cursoService;
        private readonly IProgresoService _progresoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<InscripcionesController> _logger;

        public InscripcionesController(
            IInscripcionService inscripcionService,
            ICursoService cursoService,
            IProgresoService progresoService,
            UserManager<ApplicationUser> userManager,
            ILogger<InscripcionesController> logger)
        {
            _inscripcionService = inscripcionService;
            _cursoService = cursoService;
            _progresoService = progresoService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Inscripciones/MisCursos
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> MisCursos()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var inscripciones = await _inscripcionService.ObtenerInscripcionesDeUsuarioAsync(userId);

            // Calcular progreso para cada curso
            var progresoPorCurso = new Dictionary<int, double>();
            foreach (var inscripcion in inscripciones)
            {
                var progreso = await _progresoService.ObtenerProgresoAsync(userId, inscripcion.CursoId);
                progresoPorCurso[inscripcion.CursoId] = progreso;
            }

            ViewData["ProgresoPorCurso"] = progresoPorCurso;
            return View(inscripciones);
        }

        // POST: /Inscripciones/Inscribir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            // Verificar que el curso existe y está publicado
            var curso = await _cursoService.ObtenerCursoPorIdAsync(cursoId);
            if (curso == null)
            {
                TempData["Error"] = "El curso no existe.";
                return RedirectToAction("Index", "Cursos");
            }

            if (!curso.EstaPublicado)
            {
                TempData["Error"] = "Este curso no está disponible.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // Verificar inscripción duplicada
            bool yaInscrito = await _inscripcionService.VerificarAccesoAsync(userId, cursoId);
            if (yaInscrito)
            {
                TempData["Error"] = "Ya estás inscrito en este curso.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            try
            {
                await _inscripcionService.InscribirUsuarioAsync(userId, cursoId);
                TempData["Success"] = $"¡Te has inscrito en «{curso.Titulo}» exitosamente!";
                return RedirectToAction("Confirmacion", new { cursoId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inscribir usuario {UserId} en curso {CursoId}", userId, cursoId);
                TempData["Error"] = "Ocurrió un error al procesar la inscripción. Intenta de nuevo.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }
        }

        // GET: /Inscripciones/Confirmacion?cursoId=5
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> Confirmacion(int cursoId)
        {
            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(cursoId);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Inscripciones/Cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Suscriptor,Administrador")]
        public async Task<IActionResult> Cancelar(int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            try
            {
                await _inscripcionService.CancelarInscripcionAsync(userId, cursoId);
                TempData["Success"] = "Tu inscripción ha sido cancelada.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar inscripción de {UserId} en curso {CursoId}", userId, cursoId);
                TempData["Error"] = "No se pudo cancelar la inscripción.";
            }

            return RedirectToAction("MisCursos");
        }
    }
}
