using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalMicrocursos.Models;

namespace PortalMicrocursos.Data
{
    /// <summary>
    /// Clase para inicializar datos base del sistema
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Inicializa roles, usuario administrador y datos de ejemplo
        /// </summary>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Asegurar que la base de datos está creada
            await context.Database.EnsureCreatedAsync();

            // 1. Crear roles si no existen
            await CrearRoles(roleManager);

            // 2. Crear usuario administrador si no existe
            await CrearAdministrador(userManager);

            // 3. Crear cursos y lecciones de ejemplo (opcional)
            await CrearCursosEjemplo(context);
        }

        /// <summary>
        /// Crea los roles del sistema: Invitado, Suscriptor, Administrador
        /// </summary>
        private static async Task CrearRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { RolesConstantes.Invitado, RolesConstantes.Suscriptor, RolesConstantes.Administrador };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                    Console.WriteLine($"✓ Rol creado: {rol}");
                }
            }
        }

        /// <summary>
        /// Crea un usuario administrador por defecto
        /// </summary>
        private static async Task CrearAdministrador(UserManager<ApplicationUser> userManager)
        {
            // Credenciales del administrador por defecto
            string emailAdmin = "admin@portalmicrocursos.com";
            string passwordAdmin = "Admin123"; // Cambiar en producción

            // Verificar si ya existe
            var adminExistente = await userManager.FindByEmailAsync(emailAdmin);
            if (adminExistente == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    NombreCompleto = "Administrador del Sistema",
                    FechaRegistro = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, passwordAdmin);

                if (result.Succeeded)
                {
                    // Asignar rol de Administrador
                    await userManager.AddToRoleAsync(admin, RolesConstantes.Administrador);
                    Console.WriteLine($"✓ Usuario administrador creado");
                    Console.WriteLine($"  Email: {emailAdmin}");
                    Console.WriteLine($"  Password: {passwordAdmin}");
                    Console.WriteLine($"  ⚠️ IMPORTANTE: Cambiar la contraseña en producción");
                }
                else
                {
                    Console.WriteLine($"✗ Error al crear administrador:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  - {error.Description}");
                    }
                }
            }
        }

        /// <summary>
        /// Crea cursos y lecciones de ejemplo para desarrollo
        /// </summary>
        private static async Task CrearCursosEjemplo(ApplicationDbContext context)
        {
            // Verificar si ya existen cursos
            if (await context.Cursos.AnyAsync())
            {
                return; // Ya hay cursos, no crear más
            }

            // Curso 1: Introducción a ASP.NET Core
            var curso1 = new Curso
            {
                Titulo = "Introducción a ASP.NET Core",
                Descripcion = "Aprende los fundamentos de ASP.NET Core para crear aplicaciones web modernas y escalables. Este curso cubre desde la configuración inicial hasta la creación de APIs REST.",
                Nivel = "Básico",
                Duracion = 180, // 3 horas
                EstaPublicado = true,
                FechaCreacion = DateTime.Now
            };

            context.Cursos.Add(curso1);
            await context.SaveChangesAsync();

            // Lecciones del Curso 1
            var leccionesCurso1 = new List<Leccion>
            {
                new Leccion
                {
                    Titulo = "Bienvenida al curso",
                    Contenido = "<h2>Bienvenido</h2><p>En este curso aprenderás los fundamentos de ASP.NET Core. Comenzaremos desde cero y construiremos una aplicación web completa paso a paso.</p>",
                    Orden = 1,
                    CursoId = curso1.Id
                },
                new Leccion
                {
                    Titulo = "Configuración del entorno de desarrollo",
                    Contenido = "<h2>Configuración</h2><p>Instalación de .NET SDK, Visual Studio o VS Code, y configuración de tu primer proyecto ASP.NET Core.</p>",
                    Orden = 2,
                    CursoId = curso1.Id
                },
                new Leccion
                {
                    Titulo = "Estructura de un proyecto MVC",
                    Contenido = "<h2>Arquitectura MVC</h2><p>Entendiendo el patrón Modelo-Vista-Controlador y cómo se implementa en ASP.NET Core.</p>",
                    Orden = 3,
                    CursoId = curso1.Id
                }
            };

            context.Lecciones.AddRange(leccionesCurso1);
            await context.SaveChangesAsync();

            // Curso 2: Entity Framework Core
            var curso2 = new Curso
            {
                Titulo = "Entity Framework Core - Base de Datos",
                Descripcion = "Domina Entity Framework Core para trabajar con bases de datos en .NET. Aprende sobre migraciones, consultas LINQ, y mejores prácticas.",
                Nivel = "Intermedio",
                Duracion = 240, // 4 horas
                EstaPublicado = true,
                FechaCreacion = DateTime.Now
            };

            context.Cursos.Add(curso2);
            await context.SaveChangesAsync();

            // Lecciones del Curso 2
            var leccionesCurso2 = new List<Leccion>
            {
                new Leccion
                {
                    Titulo = "¿Qué es Entity Framework Core?",
                    Contenido = "<h2>Introducción a EF Core</h2><p>Entity Framework Core es un ORM (Object-Relational Mapper) moderno y ligero para .NET.</p>",
                    Orden = 1,
                    CursoId = curso2.Id
                },
                new Leccion
                {
                    Titulo = "Code First y Migraciones",
                    Contenido = "<h2>Code First</h2><p>Aprende a crear tu base de datos desde modelos de C# usando migraciones.</p>",
                    Orden = 2,
                    CursoId = curso2.Id
                }
            };

            context.Lecciones.AddRange(leccionesCurso2);
            await context.SaveChangesAsync();

            // Curso 3: ASP.NET Core Identity
            var curso3 = new Curso
            {
                Titulo = "Seguridad con ASP.NET Core Identity",
                Descripcion = "Implementa autenticación y autorización robusta en tus aplicaciones con ASP.NET Core Identity.",
                Nivel = "Avanzado",
                Duracion = 300, // 5 horas
                EstaPublicado = false, // No publicado aún
                FechaCreacion = DateTime.Now
            };

            context.Cursos.Add(curso3);
            await context.SaveChangesAsync();

            var leccionesCurso3 = new List<Leccion>
            {
                new Leccion
                {
                    Titulo = "Fundamentos de Identity",
                    Contenido = "<h2>ASP.NET Core Identity</h2><p>Sistema completo de autenticación y autorización para aplicaciones ASP.NET Core.</p>",
                    Orden = 1,
                    CursoId = curso3.Id
                }
            };

            context.Lecciones.AddRange(leccionesCurso3);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Cursos y lecciones de ejemplo creados");
        }
    }
}
