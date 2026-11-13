using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Data
{
    /// <summary>
    /// DbContext principal que hereda de IdentityDbContext para integrar ASP.NET Core Identity
    /// Incluye todas las tablas de Identity (AspNetUsers, AspNetRoles, etc.) automáticamente
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades personalizadas del sistema
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Leccion> Lecciones { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<LeccionCompletada> LeccionesCompletadas { get; set; }

        // Los DbSets de Identity (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) 
        // se incluyen automáticamente al heredar de IdentityDbContext<ApplicationUser>

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llamar al método base para configurar Identity
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

                // Índices para búsquedas eficientes y optimización de consultas
                entity.HasIndex(c => c.Titulo).HasDatabaseName("IX_Cursos_Titulo");
                entity.HasIndex(c => c.EstaPublicado).HasDatabaseName("IX_Cursos_EstaPublicado");
                entity.HasIndex(c => c.Nivel).HasDatabaseName("IX_Cursos_Nivel");
                entity.HasIndex(c => c.FechaCreacion).HasDatabaseName("IX_Cursos_FechaCreacion");
                // Índice compuesto para filtrar cursos publicados por nivel (consulta común)
                entity.HasIndex(c => new { c.EstaPublicado, c.Nivel }).HasDatabaseName("IX_Cursos_Publicado_Nivel");
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

                // Índice compuesto único: No puede haber dos lecciones con el mismo orden en un curso
                entity.HasIndex(l => new { l.CursoId, l.Orden })
                      .IsUnique()
                      .HasDatabaseName("IX_Lecciones_Curso_Orden");
                
                // Índice en CursoId para optimizar consultas de lecciones por curso
                entity.HasIndex(l => l.CursoId).HasDatabaseName("IX_Lecciones_CursoId");
            });

            // Configuración de la entidad Inscripción
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.UsuarioId).IsRequired();
                entity.Property(i => i.CursoId).IsRequired();
                entity.Property(i => i.FechaInscripcion).HasDefaultValueSql("GETDATE()");

                // Relación: Una Inscripción pertenece a un Usuario (Identity)
                entity.HasOne(i => i.Usuario)
                      .WithMany(u => u.Inscripciones)
                      .HasForeignKey(i => i.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación: Una Inscripción pertenece a un Curso
                entity.HasOne(i => i.Curso)
                      .WithMany(c => c.Inscripciones)
                      .HasForeignKey(i => i.CursoId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único: Un usuario solo puede inscribirse una vez en un curso
                entity.HasIndex(i => new { i.UsuarioId, i.CursoId })
                      .IsUnique()
                      .HasDatabaseName("IX_Inscripciones_Usuario_Curso");
                
                // Índices individuales para optimizar consultas frecuentes
                entity.HasIndex(i => i.UsuarioId).HasDatabaseName("IX_Inscripciones_UsuarioId");
                entity.HasIndex(i => i.CursoId).HasDatabaseName("IX_Inscripciones_CursoId");
                entity.HasIndex(i => i.FechaInscripcion).HasDatabaseName("IX_Inscripciones_FechaInscripcion");
            });

            // Configuración de la entidad LecciónCompletada
            modelBuilder.Entity<LeccionCompletada>(entity =>
            {
                entity.HasKey(lc => lc.Id);
                entity.Property(lc => lc.UsuarioId).IsRequired();
                entity.Property(lc => lc.LeccionId).IsRequired();
                entity.Property(lc => lc.FechaCompletada).HasDefaultValueSql("GETDATE()");

                // Relación: Una LecciónCompletada pertenece a un Usuario (Identity)
                entity.HasOne(lc => lc.Usuario)
                      .WithMany(u => u.LeccionesCompletadas)
                      .HasForeignKey(lc => lc.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación: Una LecciónCompletada pertenece a una Lección
                entity.HasOne(lc => lc.Leccion)
                      .WithMany(l => l.LeccionesCompletadas)
                      .HasForeignKey(lc => lc.LeccionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice único: Un usuario solo puede completar una lección una vez
                entity.HasIndex(lc => new { lc.UsuarioId, lc.LeccionId })
                      .IsUnique()
                      .HasDatabaseName("IX_LeccionesCompletadas_Usuario_Leccion");
                
                // Índices individuales para optimizar cálculo de progreso y consultas
                entity.HasIndex(lc => lc.UsuarioId).HasDatabaseName("IX_LeccionesCompletadas_UsuarioId");
                entity.HasIndex(lc => lc.LeccionId).HasDatabaseName("IX_LeccionesCompletadas_LeccionId");
                entity.HasIndex(lc => lc.FechaCompletada).HasDatabaseName("IX_LeccionesCompletadas_FechaCompletada");
            });

            // Personalizar nombres de tablas de Identity (opcional)
            // Si prefieres mantener los nombres por defecto (AspNetUsers, AspNetRoles, etc.), 
            // puedes comentar o eliminar estas líneas
            /*
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Usuarios");
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Roles");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UsuariosRoles");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UsuariosClaims");
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RolesClaims");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UsuariosLogins");
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UsuariosTokens");
            });
            */
        }
    }
}
