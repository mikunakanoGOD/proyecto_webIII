using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    [Authorize(Roles = "Suscriptor,Administrador")]
    public class ProgresoController : Controller
    {
        private readonly IProgresoService _progresoService;
        private readonly ICursoService _cursoService;
        private readonly ILeccionService _leccionService;
        private readonly IInscripcionService _inscripcionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProgresoController(
            IProgresoService progresoService,
            ICursoService cursoService,
            ILeccionService leccionService,
            IInscripcionService inscripcionService,
            UserManager<ApplicationUser> userManager)
        {
            _progresoService = progresoService;
            _cursoService = cursoService;
            _leccionService = leccionService;
            _inscripcionService = inscripcionService;
            _userManager = userManager;
        }

        // GET: /Progreso/Detalle/5  (cursoId)
        public async Task<IActionResult> Detalle(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(id);
            if (curso == null) return NotFound();

            // Verificar que el usuario está inscrito (admins pueden ver todo)
            if (!User.IsInRole(RolesConstantes.Administrador))
            {
                bool inscrito = await _inscripcionService.VerificarAccesoAsync(userId, id);
                if (!inscrito)
                {
                    TempData["Error"] = "Debes estar inscrito en el curso para ver el progreso.";
                    return RedirectToAction("Detalle", "Cursos", new { id });
                }
            }

            var detalle = await _progresoService.ObtenerProgresoDetalladoAsync(userId, id);
            var lecciones = await _leccionService.ObtenerLeccionesPorCursoAsync(id);

            ViewData["Curso"] = curso;
            ViewData["Lecciones"] = lecciones;

            return View(detalle);
        }
    }
}
