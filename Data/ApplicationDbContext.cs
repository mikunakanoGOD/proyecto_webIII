using Microsoft.EntityFrameworkCore;

namespace PortalMicrocursos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Los DbSet se agregarán en la Fase 4 cuando creemos las entidades
        // Por ahora solo dejamos la configuración base

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Las configuraciones de relaciones se agregarán en fases posteriores
        }
    }
}
