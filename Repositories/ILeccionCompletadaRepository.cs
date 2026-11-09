using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de Lecciones Completadas con operaciones específicas
    /// </summary>
    public interface ILeccionCompletadaRepository : IRepository<LeccionCompletada>
    {
        /// <summary>
        /// Obtiene todas las lecciones completadas por un usuario en un curso específico
        /// </summary>
        Task<IEnumerable<LeccionCompletada>> ObtenerPorUsuarioYCursoAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Marca una lección como completada para un usuario
        /// </summary>
        Task<LeccionCompletada> MarcarComoCompletadaAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Verifica si un usuario ha completado una lección específica
        /// </summary>
        Task<bool> VerificarCompletadaAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Obtiene el registro de lección completada
        /// </summary>
        Task<LeccionCompletada?> ObtenerLeccionCompletadaAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Cuenta cuántas lecciones ha completado un usuario en un curso
        /// </summary>
        Task<int> ContarLeccionesCompletadasAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Calcula el porcentaje de progreso de un usuario en un curso
        /// </summary>
        Task<double> CalcularProgresoAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Obtiene los IDs de las lecciones completadas por un usuario en un curso
        /// </summary>
        Task<List<int>> ObtenerIdsLeccionesCompletadasAsync(string usuarioId, int cursoId);
    }
}
