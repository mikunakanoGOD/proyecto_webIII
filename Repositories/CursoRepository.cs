using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Cursos
    /// </summary>
    public class CursoRepository : Repository<Curso>, ICursoRepository
    {
        public CursoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Curso>> ObtenerCursosPublicadosAsync()
        {
            return await _dbSet
                .Where(c => c.EstaPublicado)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Curso?> ObtenerCursoConLeccionesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Lecciones)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> BuscarPorTituloAsync(string titulo)
        {
            return await _dbSet
                .Where(c => c.Titulo.Contains(titulo))
                .OrderBy(c => c.Titulo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> ObtenerPorNivelAsync(string nivel)
        {
            return await _dbSet
                .Where(c => c.Nivel == nivel && c.EstaPublicado)
                .OrderBy(c => c.Titulo)
                .ToListAsync();
        }

        public async Task PublicarCursoAsync(int id)
        {
            var curso = await ObtenerPorIdAsync(id);
            if (curso != null)
            {
                curso.EstaPublicado = true;
                await ActualizarAsync(curso);
            }
        }

        public async Task DespublicarCursoAsync(int id)
        {
            var curso = await ObtenerPorIdAsync(id);
            if (curso != null)
            {
                curso.EstaPublicado = false;
                await ActualizarAsync(curso);
            }
        }
    }
}
