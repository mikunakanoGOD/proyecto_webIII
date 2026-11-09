namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD básicas
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        Task<IEnumerable<T>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        Task<T?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        Task<T> AgregarAsync(T entidad);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        Task ActualizarAsync(T entidad);

        /// <summary>
        /// Elimina una entidad por su ID
        /// </summary>
        Task EliminarAsync(int id);

        /// <summary>
        /// Verifica si existe una entidad por su ID
        /// </summary>
        Task<bool> ExisteAsync(int id);
    }
}
