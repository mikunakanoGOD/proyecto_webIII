using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de Lecciones con operaciones específicas
    /// </summary>
    public interface ILeccionRepository : IRepository<Leccion>
    {
        /// <summary>
        /// Obtiene todas las lecciones de un curso ordenadas por Orden
        /// </summary>
        Task<IEnumerable<Leccion>> ObtenerPorCursoAsync(int cursoId);

        /// <summary>
        /// Obtiene una lección específica de un curso
        /// </summary>
        Task<Leccion?> ObtenerLeccionDeCursoAsync(int cursoId, int leccionId);

        /// <summary>
        /// Obtiene la primera lección de un curso
        /// </summary>
        Task<Leccion?> ObtenerPrimeraLeccionAsync(int cursoId);

        /// <summary>
        /// Obtiene la siguiente lección en orden
        /// </summary>
        Task<Leccion?> ObtenerSiguienteLeccionAsync(int cursoId, int ordenActual);

        /// <summary>
        /// Obtiene la lección anterior en orden
        /// </summary>
        Task<Leccion?> ObtenerLeccionAnteriorAsync(int cursoId, int ordenActual);

        /// <summary>
        /// Cuenta el número de lecciones de un curso
        /// </summary>
        Task<int> ContarLeccionesDeCursoAsync(int cursoId);

        /// <summary>
        /// Reordena las lecciones de un curso
        /// </summary>
        Task ReordenarLeccionesAsync(int cursoId, Dictionary<int, int> nuevosOrdenes);
    }
}
