using PortalMicrocursos.Models;
using PortalMicrocursos.Repositories;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Implementación del servicio de Cursos con lógica de negocio
    /// </summary>
    public class CursoService : ICursoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CursoService> _logger;

        public CursoService(IUnitOfWork unitOfWork, ILogger<CursoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosPublicadosAsync()
        {
            try
            {
                return await _unitOfWork.Cursos.ObtenerCursosPublicadosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos publicados");
                throw;
            }
        }

        public async Task<IEnumerable<Curso>> ObtenerTodosCursosAsync()
        {
            try
            {
                return await _unitOfWork.Cursos.ObtenerTodosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los cursos");
                throw;
            }
        }

        public async Task<Curso?> ObtenerCursoPorIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Cursos.ObtenerPorIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener curso con ID {CursoId}", id);
                throw;
            }
        }

        public async Task<Curso?> ObtenerCursoConLeccionesAsync(int id)
        {
            try
            {
                return await _unitOfWork.Cursos.ObtenerCursoConLeccionesAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener curso con lecciones ID {CursoId}", id);
                throw;
            }
        }

        public async Task<Curso> CrearCursoAsync(Curso curso)
        {
            try
            {
                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(curso.Titulo))
                    throw new ArgumentException("El título del curso es obligatorio");

                if (string.IsNullOrWhiteSpace(curso.Descripcion))
                    throw new ArgumentException("La descripción del curso es obligatoria");

                if (curso.Duracion <= 0)
                    throw new ArgumentException("La duración debe ser mayor a cero");

                // Establecer fecha de creación
                curso.FechaCreacion = DateTime.Now;

                // Por defecto, los cursos nuevos no están publicados
                curso.EstaPublicado = false;

                var cursoCreado = await _unitOfWork.Cursos.AgregarAsync(curso);
                
                _logger.LogInformation("Curso creado: {CursoId} - {CursoTitulo}", cursoCreado.Id, cursoCreado.Titulo);
                
                return cursoCreado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear curso: {CursoTitulo}", curso.Titulo);
                throw;
            }
        }

        public async Task ActualizarCursoAsync(Curso curso)
        {
            try
            {
                // Validar que el curso existe
                var cursoExistente = await _unitOfWork.Cursos.ObtenerPorIdAsync(curso.Id);
                if (cursoExistente == null)
                    throw new InvalidOperationException($"No existe un curso con ID {curso.Id}");

                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(curso.Titulo))
                    throw new ArgumentException("El título del curso es obligatorio");

                if (string.IsNullOrWhiteSpace(curso.Descripcion))
                    throw new ArgumentException("La descripción del curso es obligatoria");

                if (curso.Duracion <= 0)
                    throw new ArgumentException("La duración debe ser mayor a cero");

                await _unitOfWork.Cursos.ActualizarAsync(curso);
                
                _logger.LogInformation("Curso actualizado: {CursoId} - {CursoTitulo}", curso.Id, curso.Titulo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar curso: {CursoId}", curso.Id);
                throw;
            }
        }

        public async Task EliminarCursoAsync(int id)
        {
            try
            {
                // Verificar si puede eliminarse
                var puedeEliminar = await PuedeEliminarCursoAsync(id);
                if (!puedeEliminar)
                    throw new InvalidOperationException("No se puede eliminar un curso con inscripciones activas");

                await _unitOfWork.Cursos.EliminarAsync(id);
                
                _logger.LogInformation("Curso eliminado: {CursoId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar curso: {CursoId}", id);
                throw;
            }
        }

        public async Task PublicarCursoAsync(int id)
        {
            try
            {
                // Validar que el curso tiene al menos una lección
                var curso = await _unitOfWork.Cursos.ObtenerCursoConLeccionesAsync(id);
                if (curso == null)
                    throw new InvalidOperationException($"No existe un curso con ID {id}");

                if (curso.Lecciones == null || !curso.Lecciones.Any())
                    throw new InvalidOperationException("No se puede publicar un curso sin lecciones");

                await _unitOfWork.Cursos.PublicarCursoAsync(id);
                
                _logger.LogInformation("Curso publicado: {CursoId} - {CursoTitulo}", id, curso.Titulo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al publicar curso: {CursoId}", id);
                throw;
            }
        }

        public async Task DespublicarCursoAsync(int id)
        {
            try
            {
                var curso = await _unitOfWork.Cursos.ObtenerPorIdAsync(id);
                if (curso == null)
                    throw new InvalidOperationException($"No existe un curso con ID {id}");

                await _unitOfWork.Cursos.DespublicarCursoAsync(id);
                
                _logger.LogInformation("Curso despublicado: {CursoId} - {CursoTitulo}", id, curso.Titulo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al despublicar curso: {CursoId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Curso>> BuscarCursosPorTituloAsync(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                    return Enumerable.Empty<Curso>();

                return await _unitOfWork.Cursos.BuscarPorTituloAsync(titulo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar cursos por título: {Titulo}", titulo);
                throw;
            }
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosPorNivelAsync(string nivel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nivel))
                    return Enumerable.Empty<Curso>();

                return await _unitOfWork.Cursos.ObtenerPorNivelAsync(nivel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos por nivel: {Nivel}", nivel);
                throw;
            }
        }

        public async Task<bool> ExisteCursoAsync(int id)
        {
            try
            {
                return await _unitOfWork.Cursos.ExisteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de curso: {CursoId}", id);
                throw;
            }
        }

        public async Task<bool> PuedeEliminarCursoAsync(int id)
        {
            try
            {
                // Un curso puede eliminarse si no tiene inscripciones
                var cantidadInscripciones = await _unitOfWork.Inscripciones.ContarInscriptosPorCursoAsync(id);
                return cantidadInscripciones == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si puede eliminar curso: {CursoId}", id);
                throw;
            }
        }
    }
}
