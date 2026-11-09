using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Inscripciones
    /// </summary>
    public class InscripcionRepository : Repository<Inscripcion>, IInscripcionRepository
    {
        public InscripcionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerPorUsuarioAsync(string usuarioId)
        {
            return await _dbSet
                .Where(i => i.UsuarioId == usuarioId)
                .Include(i => i.Curso)
                .OrderByDescending(i => i.FechaInscripcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerPorCursoAsync(int cursoId)
        {
            return await _dbSet
                .Where(i => i.CursoId == cursoId)
                .Include(i => i.Usuario)
                .OrderByDescending(i => i.FechaInscripcion)
                .ToListAsync();
        }

        public async Task<bool> VerificarInscripcionAsync(string usuarioId, int cursoId)
        {
            return await _dbSet
                .AnyAsync(i => i.UsuarioId == usuarioId && i.CursoId == cursoId);
        }

        public async Task<Inscripcion?> ObtenerInscripcionAsync(string usuarioId, int cursoId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i => i.UsuarioId == usuarioId && i.CursoId == cursoId);
        }

        public async Task<int> ContarInscriptosPorCursoAsync(int cursoId)
        {
            return await _dbSet
                .CountAsync(i => i.CursoId == cursoId);
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosInscritosAsync(string usuarioId)
        {
            return await _dbSet
                .Where(i => i.UsuarioId == usuarioId)
                .Select(i => i.Curso!)
                .ToListAsync();
        }
    }
}
