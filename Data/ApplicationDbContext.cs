using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades del sistema
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Leccion> Lecciones { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<LeccionCompletada> LeccionesCompletadas { get; set; }

        // Nota: Los DbSets de Identity (AspNetUsers, AspNetRoles, etc.) 
        // se agregarán automáticamente en la Fase 5 cuando heredemos de IdentityDbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Curso
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Descripcion).IsRequired().HasMaxLength(2000);
                entity.Property(c => c.Nivel).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Duracion).IsRequired();
                entity.Property(c => c.EstaPublicado).HasDefaultValue(false);
                entity.Property(c => c.FechaCreacion).HasDefaultValueSql("GETDATE()");

                // Índice para búsquedas por título
                entity.HasIndex(c => c.Titulo);
                entity.HasIndex(c => c.EstaPublicado);
            });

            // Configuración de la entidad Lección
            modelBuilder.Entity<Leccion>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(l => l.Contenido).IsRequired();
                entity.Property(l => l.Orden).IsRequired();

                // Relación: Una Lección pertenece a un Curso
                entity.HasOne(l => l.Curso)
                      .WithMany(c => c.Lecciones)
                      .HasForeignKey(l => l.CursoId)
                      .OnDelete(DeleteBehavior.Cascade); // Si se borra un curso, se borran sus lecciones

                // Índice compuesto para búsquedas por curso y orden
                entity.HasIndex(l => new { l.CursoId, l.Orden }).IsUnique();
            });

            // Configuración de la entidad Inscripción
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.UsuarioId).IsRequired();
                entity.Property(i => i.CursoId).IsRequired();
                entity.Property(i => i.FechaInscripcion).HasDefaultValueSql("GETDATE()");

                // Relación: Una Inscripción pertenece a un Curso
                entity.HasOne(i => i.Curso)
                      .WithMany(c => c.Inscripciones)
                      .HasForeignKey(i => i.CursoId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único: Un usuario solo puede inscribirse una vez en un curso
                entity.HasIndex(i => new { i.UsuarioId, i.CursoId }).IsUnique();
            });

            // Configuración de la entidad LecciónCompletada
            modelBuilder.Entity<LeccionCompletada>(entity =>
            {
                entity.HasKey(lc => lc.Id);
                entity.Property(lc => lc.UsuarioId).IsRequired();
                entity.Property(lc => lc.LeccionId).IsRequired();
                entity.Property(lc => lc.FechaCompletada).HasDefaultValueSql("GETDATE()");

                // Relación: Una LecciónCompletada pertenece a una Lección
                entity.HasOne(lc => lc.Leccion)
                      .WithMany(l => l.LeccionesCompletadas)
                      .HasForeignKey(lc => lc.LeccionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único: Un usuario solo puede completar una lección una vez
                entity.HasIndex(lc => new { lc.UsuarioId, lc.LeccionId }).IsUnique();
            });

            // Nota: Las relaciones con ApplicationUser se configurarán en la Fase 5
            // cuando el DbContext herede de IdentityDbContext<ApplicationUser>
        }
    }
}
