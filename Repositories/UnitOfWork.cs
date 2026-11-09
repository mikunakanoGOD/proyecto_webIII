using Microsoft.EntityFrameworkCore.Storage;
using PortalMicrocursos.Data;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work
    /// Coordina las operaciones de múltiples repositorios en una sola transacción
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repositorios lazy-initialized
        private ICursoRepository? _cursoRepository;
        private ILeccionRepository? _leccionRepository;
        private IInscripcionRepository? _inscripcionRepository;
        private ILeccionCompletadaRepository? _leccionCompletadaRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Repositorio de Cursos (lazy loading)
        /// </summary>
        public ICursoRepository Cursos
        {
            get
            {
                _cursoRepository ??= new CursoRepository(_context);
                return _cursoRepository;
            }
        }

        /// <summary>
        /// Repositorio de Lecciones (lazy loading)
        /// </summary>
        public ILeccionRepository Lecciones
        {
            get
            {
                _leccionRepository ??= new LeccionRepository(_context);
                return _leccionRepository;
            }
        }

        /// <summary>
        /// Repositorio de Inscripciones (lazy loading)
        /// </summary>
        public IInscripcionRepository Inscripciones
        {
            get
            {
                _inscripcionRepository ??= new InscripcionRepository(_context);
                return _inscripcionRepository;
            }
        }

        /// <summary>
        /// Repositorio de Lecciones Completadas (lazy loading)
        /// </summary>
        public ILeccionCompletadaRepository LeccionesCompletadas
        {
            get
            {
                _leccionCompletadaRepository ??= new LeccionCompletadaRepository(_context);
                return _leccionCompletadaRepository;
            }
        }

        /// <summary>
        /// Guarda todos los cambios pendientes
        /// </summary>
        public async Task<int> GuardarCambiosAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        public async Task IniciarTransaccionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        public async Task ConfirmarTransaccionAsync()
        {
            try
            {
                await GuardarCambiosAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RevertirTransaccionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        public async Task RevertirTransaccionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
