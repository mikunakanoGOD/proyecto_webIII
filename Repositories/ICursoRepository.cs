using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de Cursos con operaciones específicas
    /// </summary>
    public interface ICursoRepository : IRepository<Curso>
    {
        /// <summary>
        /// Obtiene todos los cursos publicados
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerCursosPublicadosAsync();

        /// <summary>
        /// Obtiene un curso por ID incluyendo sus lecciones
        /// </summary>
        Task<Curso?> ObtenerCursoConLeccionesAsync(int id);

        /// <summary>
        /// Busca cursos por título
        /// </summary>
        Task<IEnumerable<Curso>> BuscarPorTituloAsync(string titulo);

        /// <summary>
        /// Obtiene cursos por nivel
        /// </summary>
        Task<IEnumerable<Curso>> ObtenerPorNivelAsync(string nivel);

        /// <summary>
        /// Publica un curso (cambia EstaPublicado a true)
        /// </summary>
        Task PublicarCursoAsync(int id);

        /// <summary>
        /// Despublica un curso (cambia EstaPublicado a false)
        /// </summary>
        Task DespublicarCursoAsync(int id);
    }
}
