using PortalMicrocursos.Models;
using PortalMicrocursos.Repositories;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Implementación del servicio de Progreso con lógica de negocio
    /// </summary>
    public class ProgresoService : IProgresoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProgresoService> _logger;

        public ProgresoService(IUnitOfWork unitOfWork, ILogger<ProgresoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<LeccionCompletada> MarcarLeccionCompletadaAsync(string usuarioId, int leccionId)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (leccionId <= 0)
                    throw new ArgumentException("El ID de la lección es inválido");

                // Verificar que la lección existe
                var leccion = await _unitOfWork.Lecciones.ObtenerPorIdAsync(leccionId);
                if (leccion == null)
                    throw new InvalidOperationException($"No existe una lección con ID {leccionId}");

                // Verificar que el usuario puede acceder a la lección (está inscrito en el curso)
                var puedeAcceder = await PuedeAccederLeccionAsync(usuarioId, leccionId);
                if (!puedeAcceder)
                    throw new InvalidOperationException("El usuario no tiene acceso a esta lección. Debe estar inscrito en el curso.");

                // Marcar como completada (el repositorio maneja la idempotencia)
                var leccionCompletada = await _unitOfWork.LeccionesCompletadas.MarcarComoCompletadaAsync(usuarioId, leccionId);

                _logger.LogInformation("Usuario {UsuarioId} completó lección {LeccionId} del curso {CursoId}", 
                    usuarioId, leccionId, leccion.CursoId);

                return leccionCompletada;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar lección {LeccionId} como completada para usuario {UsuarioId}", 
                    leccionId, usuarioId);
                throw;
            }
        }

        public async Task<double> ObtenerProgresoAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                return await _unitOfWork.LeccionesCompletadas.CalcularProgresoAsync(usuarioId, cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener progreso del usuario {UsuarioId} en curso {CursoId}", 
                    usuarioId, cursoId);
                throw;
            }
        }

        public async Task<ProgresoDetalle> ObtenerProgresoDetalladoAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                // Obtener curso
                var curso = await _unitOfWork.Cursos.ObtenerPorIdAsync(cursoId);
                if (curso == null)
                    throw new InvalidOperationException($"No existe un curso con ID {cursoId}");

                // Verificar inscripción
                var inscrito = await _unitOfWork.Inscripciones.VerificarInscripcionAsync(usuarioId, cursoId);
                if (!inscrito)
                    throw new InvalidOperationException("El usuario no está inscrito en este curso");

                // Obtener datos de progreso
                var totalLecciones = await _unitOfWork.Lecciones.ContarLeccionesDeCursoAsync(cursoId);
                var leccionesCompletadas = await _unitOfWork.LeccionesCompletadas.ContarLeccionesCompletadasAsync(usuarioId, cursoId);
                var idsCompletadas = await _unitOfWork.LeccionesCompletadas.ObtenerIdsLeccionesCompletadasAsync(usuarioId, cursoId);
                var progreso = await _unitOfWork.LeccionesCompletadas.CalcularProgresoAsync(usuarioId, cursoId);

                var detalle = new ProgresoDetalle
                {
                    CursoId = cursoId,
                    CursoTitulo = curso.Titulo,
                    TotalLecciones = totalLecciones,
                    LeccionesCompletadas = leccionesCompletadas,
                    PorcentajeProgreso = progreso,
                    IdsLeccionesCompletadas = idsCompletadas,
                    CursoCompletado = progreso >= 100
                };

                return detalle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener progreso detallado del usuario {UsuarioId} en curso {CursoId}", 
                    usuarioId, cursoId);
                throw;
            }
        }

        public async Task<bool> EstaLeccionCompletadaAsync(string usuarioId, int leccionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId) || leccionId <= 0)
                    return false;

                return await _unitOfWork.LeccionesCompletadas.VerificarCompletadaAsync(usuarioId, leccionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si lección {LeccionId} está completada para usuario {UsuarioId}", 
                    leccionId, usuarioId);
                throw;
            }
        }

        public async Task<bool> PuedeAccederLeccionAsync(string usuarioId, int leccionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId) || leccionId <= 0)
                    return false;

                // Obtener la lección para conocer su curso
                var leccion = await _unitOfWork.Lecciones.ObtenerPorIdAsync(leccionId);
                if (leccion == null)
                    return false;

                // Verificar si el usuario está inscrito en el curso de la lección
                return await _unitOfWork.Inscripciones.VerificarInscripcionAsync(usuarioId, leccion.CursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar acceso a lección {LeccionId} para usuario {UsuarioId}", 
                    leccionId, usuarioId);
                throw;
            }
        }

        public async Task<List<int>> ObtenerLeccionesCompletadasAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                return await _unitOfWork.LeccionesCompletadas.ObtenerIdsLeccionesCompletadasAsync(usuarioId, cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones completadas del usuario {UsuarioId} en curso {CursoId}", 
                    usuarioId, cursoId);
                throw;
            }
        }

        public async Task<bool> HaCompletadoCursoAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId) || cursoId <= 0)
                    return false;

                var progreso = await _unitOfWork.LeccionesCompletadas.CalcularProgresoAsync(usuarioId, cursoId);
                return progreso >= 100;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si usuario {UsuarioId} completó curso {CursoId}", 
                    usuarioId, cursoId);
                throw;
            }
        }
    }
}
