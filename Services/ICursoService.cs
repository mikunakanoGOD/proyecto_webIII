using PortalMicrocursos.Models;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Interfaz del servicio de Cursos con lógica de negocio
    /// </summary>
    public interface ICursoService
    {
        /// <summary>
        /// Obtiene todos los cursos publicados disponibles para visualización pública
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerCursosPublicadosAsync();

        /// <summary>
        /// Obtiene todos los cursos (publicados y no publicados) para administración
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerTodosCursosAsync();

        /// <summary>
        /// Obtiene un curso por su ID
        /// </summary>
        Task<Curso?> ObtenerCursoPorIdAsync(int id);

        /// <summary>
        /// Obtiene un curso con todas sus lecciones
        /// </summary>
        Task<Curso?> ObtenerCursoConLeccionesAsync(int id);

        /// <summary>
        /// Crea un nuevo curso
        /// </summary>
        Task<Curso> CrearCursoAsync(Curso curso);

        /// <summary>
        /// Actualiza un curso existente
        /// </summary>
        Task ActualizarCursoAsync(Curso curso);

        /// <summary>
        /// Elimina un curso y todas sus lecciones asociadas
        /// </summary>
        Task EliminarCursoAsync(int id);

        /// <summary>
        /// Publica un curso (lo hace visible al público)
        /// </summary>
        Task PublicarCursoAsync(int id);

        /// <summary>
        /// Despublica un curso (lo oculta del público)
        /// </summary>
        Task DespublicarCursoAsync(int id);

        /// <summary>
        /// Busca cursos por título
        /// </summary>
        Task<IEnumerable<Curso>> BuscarCursosPorTituloAsync(string titulo);

        /// <summary>
        /// Obtiene cursos filtrados por nivel
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerCursosPorNivelAsync(string nivel);

        /// <summary>
        /// Verifica si un curso existe
        /// </summary>
        Task<bool> ExisteCursoAsync(int id);

        /// <summary>
        /// Verifica si un curso puede ser eliminado (no tiene inscripciones)
        /// </summary>
        Task<bool> PuedeEliminarCursoAsync(int id);
    }
}
