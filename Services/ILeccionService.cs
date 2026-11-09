using PortalMicrocursos.Models;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Interfaz del servicio de Lecciones con lógica de negocio
    /// </summary>
    public interface ILeccionService
    {
        /// <summary>
        /// Obtiene todas las lecciones de un curso ordenadas
        /// </summary>
        Task<IEnumerable<Leccion>> ObtenerLeccionesPorCursoAsync(int cursoId);

        /// <summary>
        /// Obtiene una lección por su ID
        /// </summary>
        Task<Leccion?> ObtenerLeccionPorIdAsync(int id);

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
        /// Crea una nueva lección
        /// </summary>
        Task<Leccion> CrearLeccionAsync(Leccion leccion);

        /// <summary>
        /// Actualiza una lección existente
        /// </summary>
        Task ActualizarLeccionAsync(Leccion leccion);

        /// <summary>
        /// Elimina una lección
        /// </summary>
        Task EliminarLeccionAsync(int id);

        /// <summary>
        /// Cuenta el número de lecciones de un curso
        /// </summary>
        Task<int> ContarLeccionesAsync(int cursoId);

        /// <summary>
        /// Reordena las lecciones de un curso
        /// </summary>
        Task ReordenarLeccionesAsync(int cursoId, Dictionary<int, int> nuevosOrdenes);

        /// <summary>
        /// Verifica si una lección existe
        /// </summary>
        Task<bool> ExisteLeccionAsync(int id);

        /// <summary>
        /// Obtiene el número de orden sugerido para una nueva lección
        /// </summary>
        Task<int> ObtenerSiguienteOrdenAsync(int cursoId);
    }
}
