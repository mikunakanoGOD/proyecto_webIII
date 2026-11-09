using PortalMicrocursos.Models;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Interfaz del servicio de Inscripciones con lógica de negocio
    /// </summary>
    public interface IInscripcionService
    {
        /// <summary>
        /// Inscribe a un usuario en un curso
        /// </summary>
        Task<Inscripcion> InscribirUsuarioAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Obtiene todos los cursos en los que está inscrito un usuario
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerCursosInscritosAsync(string usuarioId);

        /// <summary>
        /// Obtiene todas las inscripciones de un usuario con detalles
        /// </summary>
        Task<IEnumerable<Inscripcion>> ObtenerInscripcionesDeUsuarioAsync(string usuarioId);

        /// <summary>
        /// Obtiene todos los suscriptores inscritos en un curso
        /// </summary>
        Task<IEnumerable<Inscripcion>> ObtenerSuscriptoresPorCursoAsync(int cursoId);

        /// <summary>
        /// Verifica si un usuario puede acceder a un curso (está inscrito)
        /// </summary>
        Task<bool> VerificarAccesoAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Cuenta el número de suscriptores inscritos en un curso
        /// </summary>
        Task<int> ContarInscriptosAsync(int cursoId);

        /// <summary>
        /// Cancela la inscripción de un usuario en un curso
        /// </summary>
        Task CancelarInscripcionAsync(string usuarioId, int cursoId);
    }
}
