using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Lecciones
    /// </summary>
    public class LeccionRepository : Repository<Leccion>, ILeccionRepository
    {
        public LeccionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Leccion>> ObtenerPorCursoAsync(int cursoId)
        {
            return await _dbSet
                .Where(l => l.CursoId == cursoId)
                .OrderBy(l => l.Orden)
                .ToListAsync();
        }

        public async Task<Leccion?> ObtenerLeccionDeCursoAsync(int cursoId, int leccionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(l => l.Id == leccionId && l.CursoId == cursoId);
        }

        public async Task<Leccion?> ObtenerPrimeraLeccionAsync(int cursoId)
        {
            return await _dbSet
                .Where(l => l.CursoId == cursoId)
                .OrderBy(l => l.Orden)
                .FirstOrDefaultAsync();
        }

        public async Task<Leccion?> ObtenerSiguienteLeccionAsync(int cursoId, int ordenActual)
        {
            return await _dbSet
                .Where(l => l.CursoId == cursoId && l.Orden > ordenActual)
                .OrderBy(l => l.Orden)
                .FirstOrDefaultAsync();
        }

        public async Task<Leccion?> ObtenerLeccionAnteriorAsync(int cursoId, int ordenActual)
        {
            return await _dbSet
                .Where(l => l.CursoId == cursoId && l.Orden < ordenActual)
                .OrderByDescending(l => l.Orden)
                .FirstOrDefaultAsync();
        }

        public async Task<int> ContarLeccionesDeCursoAsync(int cursoId)
        {
            return await _dbSet
                .CountAsync(l => l.CursoId == cursoId);
        }

        public async Task ReordenarLeccionesAsync(int cursoId, Dictionary<int, int> nuevosOrdenes)
        {
            var lecciones = await _dbSet
                .Where(l => l.CursoId == cursoId)
                .ToListAsync();

            foreach (var leccion in lecciones)
            {
                if (nuevosOrdenes.ContainsKey(leccion.Id))
                {
                    leccion.Orden = nuevosOrdenes[leccion.Id];
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
