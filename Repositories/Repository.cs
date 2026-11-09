using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación genérica del repositorio con operaciones CRUD básicas
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> AgregarAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public virtual async Task ActualizarAsync(T entidad)
        {
            _dbSet.Update(entidad);
            await _context.SaveChangesAsync();
        }

        public virtual async Task EliminarAsync(int id)
        {
            var entidad = await ObtenerPorIdAsync(id);
            if (entidad != null)
            {
                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExisteAsync(int id)
        {
            var entidad = await ObtenerPorIdAsync(id);
            return entidad != null;
        }
    }
}
