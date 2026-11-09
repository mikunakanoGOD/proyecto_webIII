using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de Inscripciones con operaciones específicas
    /// </summary>
    public interface IInscripcionRepository : IRepository<Inscripcion>
    {
        /// <summary>
        /// Obtiene todas las inscripciones de un usuario
        /// </summary>
        Task<IEnumerable<Inscripcion>> ObtenerPorUsuarioAsync(string usuarioId);

        /// <summary>
        /// Obtiene todas las inscripciones de un curso con información del usuario
        /// </summary>
        Task<IEnumerable<Inscripcion>> ObtenerPorCursoAsync(int cursoId);

        /// <summary>
        /// Verifica si un usuario está inscrito en un curso
        /// </summary>
        Task<bool> VerificarInscripcionAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Obtiene una inscripción específica de usuario y curso
        /// </summary>
        Task<Inscripcion?> ObtenerInscripcionAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Cuenta el número de suscriptores inscritos en un curso
        /// </summary>
        Task<int> ContarInscriptosPorCursoAsync(int cursoId);

        /// <summary>
        /// Obtiene los cursos en los que está inscrito un usuario con detalles del curso
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerCursosInscritosAsync(string usuarioId);
    }
}
