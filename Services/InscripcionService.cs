using PortalMicrocursos.Models;
using PortalMicrocursos.Repositories;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Implementación del servicio de Inscripciones con lógica de negocio
    /// </summary>
    public class InscripcionService : IInscripcionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InscripcionService> _logger;

        public InscripcionService(IUnitOfWork unitOfWork, ILogger<InscripcionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Inscripcion> InscribirUsuarioAsync(string usuarioId, int cursoId)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                // Verificar que el curso existe y está publicado
                var curso = await _unitOfWork.Cursos.ObtenerPorIdAsync(cursoId);
                if (curso == null)
                    throw new InvalidOperationException($"No existe un curso con ID {cursoId}");

                if (!curso.EstaPublicado)
                    throw new InvalidOperationException("No se puede inscribir en un curso no publicado");

                // Verificar que el usuario no esté ya inscrito
                var yaInscrito = await _unitOfWork.Inscripciones.VerificarInscripcionAsync(usuarioId, cursoId);
                if (yaInscrito)
                    throw new InvalidOperationException("El usuario ya está inscrito en este curso");

                // Crear inscripción
                var inscripcion = new Inscripcion
                {
                    UsuarioId = usuarioId,
                    CursoId = cursoId,
                    FechaInscripcion = DateTime.Now
                };

                var inscripcionCreada = await _unitOfWork.Inscripciones.AgregarAsync(inscripcion);

                _logger.LogInformation("Usuario {UsuarioId} inscrito en curso {CursoId} - {CursoTitulo}", 
                    usuarioId, cursoId, curso.Titulo);

                return inscripcionCreada;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inscribir usuario {UsuarioId} en curso {CursoId}", usuarioId, cursoId);
                throw;
            }
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosInscritosAsync(string usuarioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                return await _unitOfWork.Inscripciones.ObtenerCursosInscritosAsync(usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos inscritos del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerInscripcionesDeUsuarioAsync(string usuarioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                return await _unitOfWork.Inscripciones.ObtenerPorUsuarioAsync(usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inscripciones del usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerSuscriptoresPorCursoAsync(int cursoId)
        {
            try
            {
                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                return await _unitOfWork.Inscripciones.ObtenerPorCursoAsync(cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener suscriptores del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task<bool> VerificarAccesoAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId) || cursoId <= 0)
                    return false;

                return await _unitOfWork.Inscripciones.VerificarInscripcionAsync(usuarioId, cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar acceso del usuario {UsuarioId} al curso {CursoId}", usuarioId, cursoId);
                throw;
            }
        }

        public async Task<int> ContarInscriptosAsync(int cursoId)
        {
            try
            {
                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                return await _unitOfWork.Inscripciones.ContarInscriptosPorCursoAsync(cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar inscritos del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task CancelarInscripcionAsync(string usuarioId, int cursoId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioId))
                    throw new ArgumentException("El ID del usuario es requerido");

                if (cursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                // Obtener la inscripción
                var inscripcion = await _unitOfWork.Inscripciones.ObtenerInscripcionAsync(usuarioId, cursoId);
                if (inscripcion == null)
                    throw new InvalidOperationException("No existe una inscripción para este usuario y curso");

                // Eliminar la inscripción
                await _unitOfWork.Inscripciones.EliminarAsync(inscripcion.Id);

                _logger.LogInformation("Inscripción cancelada: Usuario {UsuarioId} en curso {CursoId}", usuarioId, cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar inscripción del usuario {UsuarioId} en curso {CursoId}", usuarioId, cursoId);
                throw;
            }
        }
    }
}
