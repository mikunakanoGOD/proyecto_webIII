using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ICursoService _cursoService;
        private readonly ILeccionService _leccionService;
        private readonly IInscripcionService _inscripcionService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ICursoService cursoService,
            ILeccionService leccionService,
            IInscripcionService inscripcionService,
            ILogger<AdminController> logger)
        {
            _cursoService = cursoService;
            _leccionService = leccionService;
            _inscripcionService = inscripcionService;
            _logger = logger;
        }

        // ============================
        // PANEL PRINCIPAL
        // ============================

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var cursos = await _cursoService.ObtenerTodosCursosAsync();
            var cursosPublicados = cursos.Count(c => c.EstaPublicado);
            var cursosBorrador = cursos.Count(c => !c.EstaPublicado);

            ViewData["TotalCursos"] = cursos.Count();
            ViewData["CursosPublicados"] = cursosPublicados;
            ViewData["CursosBorrador"] = cursosBorrador;

            return View(cursos);
        }

        // ============================
        // GESTIÓN DE CURSOS
        // ============================

        // GET: /Admin/Cursos
        public async Task<IActionResult> Cursos()
        {
            var cursos = await _cursoService.ObtenerTodosCursosAsync();
            return View(cursos);
        }

        // GET: /Admin/CrearCurso
        [HttpGet]
        public IActionResult CrearCurso()
        {
            return View(new Curso());
        }

        // POST: /Admin/CrearCurso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCurso(Curso curso)
        {
            if (!ModelState.IsValid)
                return View(curso);

            try
            {
                await _cursoService.CrearCursoAsync(curso);
                TempData["Success"] = $"El curso «{curso.Titulo}» fue creado exitosamente.";
                return RedirectToAction("Cursos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear curso");
                ModelState.AddModelError(string.Empty, "Error al guardar el curso: " + ex.Message);
                return View(curso);
            }
        }

        // GET: /Admin/EditarCurso/5
        [HttpGet]
        public async Task<IActionResult> EditarCurso(int id)
        {
            var curso = await _cursoService.ObtenerCursoPorIdAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Admin/EditarCurso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCurso(int id, Curso curso)
        {
            if (id != curso.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(curso);

            try
            {
                await _cursoService.ActualizarCursoAsync(curso);
                TempData["Success"] = $"El curso «{curso.Titulo}» fue actualizado correctamente.";
                return RedirectToAction("Cursos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar curso {Id}", id);
                ModelState.AddModelError(string.Empty, "Error al actualizar: " + ex.Message);
                return View(curso);
            }
        }

        // GET: /Admin/EliminarCurso/5
        [HttpGet]
        public async Task<IActionResult> EliminarCurso(int id)
        {
            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(id);
            if (curso == null) return NotFound();

            bool puedeEliminar = await _cursoService.PuedeEliminarCursoAsync(id);
            ViewData["PuedeEliminar"] = puedeEliminar;

            return View(curso);
        }

        // POST: /Admin/EliminarCurso/5
        [HttpPost, ActionName("EliminarCurso")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCursoConfirmado(int id)
        {
            try
            {
                var curso = await _cursoService.ObtenerCursoPorIdAsync(id);
                string titulo = curso?.Titulo ?? "Sin título";
                await _cursoService.EliminarCursoAsync(id);
                TempData["Success"] = $"El curso «{titulo}» fue eliminado.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar curso {Id}", id);
                TempData["Error"] = "Error al eliminar el curso.";
            }

            return RedirectToAction("Cursos");
        }

        // POST: /Admin/PublicarCurso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublicarCurso(int id)
        {
            try
            {
                await _cursoService.PublicarCursoAsync(id);
                TempData["Success"] = "El curso fue publicado y está visible para los usuarios.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al publicar curso {Id}", id);
                TempData["Error"] = "Error al publicar el curso.";
            }

            return RedirectToAction("Cursos");
        }

        // POST: /Admin/DespublicarCurso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DespublicarCurso(int id)
        {
            try
            {
                await _cursoService.DespublicarCursoAsync(id);
                TempData["Success"] = "El curso fue despublicado y ya no es visible para los usuarios.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al despublicar curso {Id}", id);
                TempData["Error"] = "Error al despublicar el curso.";
            }

            return RedirectToAction("Cursos");
        }

        // ============================
        // GESTIÓN DE LECCIONES
        // ============================

        // GET: /Admin/Lecciones/5  (cursoId)
        public async Task<IActionResult> Lecciones(int id)
        {
            var curso = await _cursoService.ObtenerCursoConLeccionesAsync(id);
            if (curso == null) return NotFound();
            ViewData["Curso"] = curso;
            var lecciones = await _leccionService.ObtenerLeccionesPorCursoAsync(id);
            return View(lecciones);
        }

        // GET: /Admin/CrearLeccion/5 (cursoId)
        [HttpGet]
        public async Task<IActionResult> CrearLeccion(int id)
        {
            var curso = await _cursoService.ObtenerCursoPorIdAsync(id);
            if (curso == null) return NotFound();

            int siguienteOrden = await _leccionService.ObtenerSiguienteOrdenAsync(id);

            ViewData["Curso"] = curso;
            return View(new Leccion { CursoId = id, Orden = siguienteOrden });
        }

        // POST: /Admin/CrearLeccion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearLeccion(Leccion leccion)
        {
            ModelState.Remove("Curso");

            if (!ModelState.IsValid)
            {
                var curso = await _cursoService.ObtenerCursoPorIdAsync(leccion.CursoId);
                ViewData["Curso"] = curso;
                return View(leccion);
            }

            try
            {
                await _leccionService.CrearLeccionAsync(leccion);
                TempData["Success"] = $"La lección «{leccion.Titulo}» fue creada exitosamente.";
                return RedirectToAction("Lecciones", new { id = leccion.CursoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear lección");
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                var curso = await _cursoService.ObtenerCursoPorIdAsync(leccion.CursoId);
                ViewData["Curso"] = curso;
                return View(leccion);
            }
        }

        // GET: /Admin/EditarLeccion/5
        [HttpGet]
        public async Task<IActionResult> EditarLeccion(int id)
        {
            var leccion = await _leccionService.ObtenerLeccionPorIdAsync(id);
            if (leccion == null) return NotFound();

            var curso = await _cursoService.ObtenerCursoPorIdAsync(leccion.CursoId);
            ViewData["Curso"] = curso;
            return View(leccion);
        }

        // POST: /Admin/EditarLeccion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarLeccion(int id, Leccion leccion)
        {
            if (id != leccion.Id) return BadRequest();

            ModelState.Remove("Curso");

            if (!ModelState.IsValid)
            {
                var curso = await _cursoService.ObtenerCursoPorIdAsync(leccion.CursoId);
                ViewData["Curso"] = curso;
                return View(leccion);
            }

            try
            {
                await _leccionService.ActualizarLeccionAsync(leccion);
                TempData["Success"] = $"La lección «{leccion.Titulo}» fue actualizada.";
                return RedirectToAction("Lecciones", new { id = leccion.CursoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar lección {Id}", id);
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                var curso = await _cursoService.ObtenerCursoPorIdAsync(leccion.CursoId);
                ViewData["Curso"] = curso;
                return View(leccion);
            }
        }

        // POST: /Admin/EliminarLeccion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarLeccion(int id, int cursoId)
        {
            try
            {
                var leccion = await _leccionService.ObtenerLeccionPorIdAsync(id);
                string titulo = leccion?.Titulo ?? "Sin título";
                await _leccionService.EliminarLeccionAsync(id);
                TempData["Success"] = $"La lección «{titulo}» fue eliminada.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lección {Id}", id);
                TempData["Error"] = "Error al eliminar la lección.";
            }

            return RedirectToAction("Lecciones", new { id = cursoId });
        }

        // ============================
        // CONSULTAS ADMINISTRATIVAS
        // ============================

        // GET: /Admin/Suscriptores
        public async Task<IActionResult> Suscriptores()
        {
            var cursos = await _cursoService.ObtenerTodosCursosAsync();
            return View(cursos);
        }

        // GET: /Admin/SuscriptoresPorCurso/5
        public async Task<IActionResult> SuscriptoresPorCurso(int id)
        {
            var curso = await _cursoService.ObtenerCursoPorIdAsync(id);
            if (curso == null) return NotFound();

            var inscripciones = await _inscripcionService.ObtenerSuscriptoresPorCursoAsync(id);
            ViewData["Curso"] = curso;
            return View(inscripciones);
        }
    }
}
