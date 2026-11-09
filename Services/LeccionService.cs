using PortalMicrocursos.Models;
using PortalMicrocursos.Repositories;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Implementación del servicio de Lecciones con lógica de negocio
    /// </summary>
    public class LeccionService : ILeccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LeccionService> _logger;

        public LeccionService(IUnitOfWork unitOfWork, ILogger<LeccionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int cursoId)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerPorCursoAsync(cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lecciones del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task<Leccion?> ObtenerLeccionPorIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerPorIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lección con ID {LeccionId}", id);
                throw;
            }
        }

        public async Task<Leccion?> ObtenerLeccionDeCursoAsync(int cursoId, int leccionId)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerLeccionDeCursoAsync(cursoId, leccionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lección {LeccionId} del curso {CursoId}", leccionId, cursoId);
                throw;
            }
        }

        public async Task<Leccion?> ObtenerPrimeraLeccionAsync(int cursoId)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerPrimeraLeccionAsync(cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener primera lección del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task<Leccion?> ObtenerSiguienteLeccionAsync(int cursoId, int ordenActual)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerSiguienteLeccionAsync(cursoId, ordenActual);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener siguiente lección del curso {CursoId} después del orden {Orden}", cursoId, ordenActual);
                throw;
            }
        }

        public async Task<Leccion?> ObtenerLeccionAnteriorAsync(int cursoId, int ordenActual)
        {
            try
            {
                return await _unitOfWork.Lecciones.ObtenerLeccionAnteriorAsync(cursoId, ordenActual);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lección anterior del curso {CursoId} antes del orden {Orden}", cursoId, ordenActual);
                throw;
            }
        }

        public async Task<Leccion> CrearLeccionAsync(Leccion leccion)
        {
            try
            {
                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(leccion.Titulo))
                    throw new ArgumentException("El título de la lección es obligatorio");

                if (string.IsNullOrWhiteSpace(leccion.Contenido))
                    throw new ArgumentException("El contenido de la lección es obligatorio");

                if (leccion.CursoId <= 0)
                    throw new ArgumentException("El ID del curso es inválido");

                // Verificar que el curso existe
                var cursoExiste = await _unitOfWork.Cursos.ExisteAsync(leccion.CursoId);
                if (!cursoExiste)
                    throw new InvalidOperationException($"No existe un curso con ID {leccion.CursoId}");

                // Si no se especificó orden, asignar el siguiente disponible
                if (leccion.Orden <= 0)
                {
                    leccion.Orden = await ObtenerSiguienteOrdenAsync(leccion.CursoId);
                }

                var leccionCreada = await _unitOfWork.Lecciones.AgregarAsync(leccion);
                
                _logger.LogInformation("Lección creada: {LeccionId} - {LeccionTitulo} en curso {CursoId}", 
                    leccionCreada.Id, leccionCreada.Titulo, leccionCreada.CursoId);
                
                return leccionCreada;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear lección: {LeccionTitulo}", leccion.Titulo);
                throw;
            }
        }

        public async Task ActualizarLeccionAsync(Leccion leccion)
        {
            try
            {
                // Validar que la lección existe
                var leccionExistente = await _unitOfWork.Lecciones.ObtenerPorIdAsync(leccion.Id);
                if (leccionExistente == null)
                    throw new InvalidOperationException($"No existe una lección con ID {leccion.Id}");

                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(leccion.Titulo))
                    throw new ArgumentException("El título de la lección es obligatorio");

                if (string.IsNullOrWhiteSpace(leccion.Contenido))
                    throw new ArgumentException("El contenido de la lección es obligatorio");

                if (leccion.Orden <= 0)
                    throw new ArgumentException("El orden debe ser mayor a cero");

                await _unitOfWork.Lecciones.ActualizarAsync(leccion);
                
                _logger.LogInformation("Lección actualizada: {LeccionId} - {LeccionTitulo}", leccion.Id, leccion.Titulo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar lección: {LeccionId}", leccion.Id);
                throw;
            }
        }

        public async Task EliminarLeccionAsync(int id)
        {
            try
            {
                var leccion = await _unitOfWork.Lecciones.ObtenerPorIdAsync(id);
                if (leccion == null)
                    throw new InvalidOperationException($"No existe una lección con ID {id}");

                await _unitOfWork.Lecciones.EliminarAsync(id);
                
                _logger.LogInformation("Lección eliminada: {LeccionId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lección: {LeccionId}", id);
                throw;
            }
        }

        public async Task<int> ContarLeccionesAsync(int cursoId)
        {
            try
            {
                return await _unitOfWork.Lecciones.ContarLeccionesDeCursoAsync(cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al contar lecciones del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task ReordenarLeccionesAsync(int cursoId, Dictionary<int, int> nuevosOrdenes)
        {
            try
            {
                if (nuevosOrdenes == null || !nuevosOrdenes.Any())
                    throw new ArgumentException("Debe proporcionar al menos un orden nuevo");

                // Validar que todas las lecciones pertenecen al curso
                foreach (var leccionId in nuevosOrdenes.Keys)
                {
                    var leccion = await _unitOfWork.Lecciones.ObtenerLeccionDeCursoAsync(cursoId, leccionId);
                    if (leccion == null)
                        throw new InvalidOperationException($"La lección {leccionId} no pertenece al curso {cursoId}");
                }

                await _unitOfWork.Lecciones.ReordenarLeccionesAsync(cursoId, nuevosOrdenes);
                
                _logger.LogInformation("Lecciones reordenadas en el curso {CursoId}", cursoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reordenar lecciones del curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task<bool> ExisteLeccionAsync(int id)
        {
            try
            {
                return await _unitOfWork.Lecciones.ExisteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de lección: {LeccionId}", id);
                throw;
            }
        }

        public async Task<int> ObtenerSiguienteOrdenAsync(int cursoId)
        {
            try
            {
                var totalLecciones = await _unitOfWork.Lecciones.ContarLeccionesDeCursoAsync(cursoId);
                return totalLecciones + 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener siguiente orden para el curso {CursoId}", cursoId);
                throw;
            }
        }
    }
}
