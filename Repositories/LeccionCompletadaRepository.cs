using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Data;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Lecciones Completadas
    /// </summary>
    public class LeccionCompletadaRepository : Repository<LeccionCompletada>, ILeccionCompletadaRepository
    {
        public LeccionCompletadaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LeccionCompletada>> ObtenerPorUsuarioYCursoAsync(string usuarioId, int cursoId)
        {
            return await _dbSet
                .Include(lc => lc.Leccion)
                .Where(lc => lc.UsuarioId == usuarioId && lc.Leccion!.CursoId == cursoId)
                .OrderBy(lc => lc.FechaCompletada)
                .ToListAsync();
        }

        public async Task<LeccionCompletada> MarcarComoCompletadaAsync(string usuarioId, int leccionId)
        {
            // Verificar si ya está completada
            var yaCompletada = await VerificarCompletadaAsync(usuarioId, leccionId);
            
            if (yaCompletada)
            {
                // Si ya está completada, retornar el registro existente
                var existente = await ObtenerLeccionCompletadaAsync(usuarioId, leccionId);
                return existente!;
            }

            // Crear nuevo registro de lección completada
            var leccionCompletada = new LeccionCompletada
            {
                UsuarioId = usuarioId,
                LeccionId = leccionId,
                FechaCompletada = DateTime.Now
            };

            return await AgregarAsync(leccionCompletada);
        }

        public async Task<bool> VerificarCompletadaAsync(string usuarioId, int leccionId)
        {
            return await _dbSet
                .AnyAsync(lc => lc.UsuarioId == usuarioId && lc.LeccionId == leccionId);
        }

        public async Task<LeccionCompletada?> ObtenerLeccionCompletadaAsync(string usuarioId, int leccionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(lc => lc.UsuarioId == usuarioId && lc.LeccionId == leccionId);
        }

        public async Task<int> ContarLeccionesCompletadasAsync(string usuarioId, int cursoId)
        {
            return await _dbSet
                .Include(lc => lc.Leccion)
                .CountAsync(lc => lc.UsuarioId == usuarioId && lc.Leccion!.CursoId == cursoId);
        }

        public async Task<double> CalcularProgresoAsync(string usuarioId, int cursoId)
        {
            // Contar total de lecciones del curso
            var totalLecciones = await _context.Lecciones
                .CountAsync(l => l.CursoId == cursoId);

            if (totalLecciones == 0)
                return 0;

            // Contar lecciones completadas
            var leccionesCompletadas = await ContarLeccionesCompletadasAsync(usuarioId, cursoId);

            // Calcular porcentaje
            return (double)leccionesCompletadas / totalLecciones * 100;
        }

        public async Task<List<int>> ObtenerIdsLeccionesCompletadasAsync(string usuarioId, int cursoId)
        {
            return await _dbSet
                .Include(lc => lc.Leccion)
                .Where(lc => lc.UsuarioId == usuarioId && lc.Leccion!.CursoId == cursoId)
                .Select(lc => lc.LeccionId)
                .ToListAsync();
        }
    }
}
