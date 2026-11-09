using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PortalMicrocursos.Models;
using PortalMicrocursos.Services;

namespace PortalMicrocursos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICursoService _cursoService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ICursoService cursoService, ILogger<HomeController> logger)
        {
            _cursoService = cursoService;
            _logger = logger;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var cursos = await _cursoService.ObtenerCursosPublicadosAsync();
            return View(cursos.Take(6)); // Mostrar últimos 6 en el home
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
