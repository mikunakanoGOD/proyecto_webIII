namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Interfaz del patrón Unit of Work
    /// Coordina las transacciones entre múltiples repositorios
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repositorio de Cursos
        /// </summary>
        ICursoRepository Cursos { get; }

        /// <summary>
        /// Repositorio de Lecciones
        /// </summary>
        ILeccionRepository Lecciones { get; }

        /// <summary>
        /// Repositorio de Inscripciones
        /// </summary>
        IInscripcionRepository Inscripciones { get; }

        /// <summary>
        /// Repositorio de Lecciones Completadas
        /// </summary>
        ILeccionCompletadaRepository LeccionesCompletadas { get; }

        /// <summary>
        /// Guarda todos los cambios pendientes en una transacción
        /// </summary>
        Task<int> GuardarCambiosAsync();

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        Task IniciarTransaccionAsync();

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        Task ConfirmarTransaccionAsync();

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        Task RevertirTransaccionAsync();
    }
}
