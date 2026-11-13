# PortalMicrocursos

Portal web de microcursos construido con **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core** y **ASP.NET Core Identity**.

## Equipo

| Integrante | Rol |
|------------|-----|
| Abigael Erquica Carvajal | Frontend |
| Edwin Miguel Zambrana Siles | Backend |

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server / SQL Server LocalDB (incluido con Visual Studio)
- Visual Studio 2022 o superior (recomendado) o VS Code

## Configuración inicial

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd proyecto_webIII
```

### 2. Configurar la cadena de conexión

El archivo `appsettings.json` ya tiene configurada la cadena por defecto para **LocalDB**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PortalMicursosDB;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

Si usas SQL Server Express, cámbiala a:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PortalMicursosDB;Integrated Security=True;TrustServerCertificate=True"
```

### 3. Aplicar las migraciones

```bash
dotnet ef database update
```

### 4. Ejecutar el proyecto

```bash
dotnet run
```

La primera ejecución ejecuta **SeedData** automáticamente y crea:
- Los 3 roles: `Invitado`, `Suscriptor`, `Administrador`
- Un usuario administrador de prueba
- 3 cursos de ejemplo con lecciones

## Credenciales de prueba

| Rol           | Correo                           | Contraseña |
|---------------|----------------------------------|------------|
| Administrador | admin@portalmicursos.com         | Admin123   |
| Suscriptor    | (crea una cuenta desde Register) | —          |

> ⚠️ **Importante:** Cambia la contraseña del administrador en producción.

## Estructura del proyecto

```
PortalMicrocursos/
├── Controllers/
│   ├── HomeController.cs          # Página principal con cursos destacados
│   ├── AccountController.cs       # Login, Register, Logout, AccessDenied
│   ├── CursosController.cs        # Catálogo público y detalle de curso
│   ├── InscripcionesController.cs # Inscripción y Mis Cursos
│   ├── LeccionesController.cs     # Visualización de lecciones
│   ├── ProgresoController.cs      # Seguimiento de progreso
│   └── AdminController.cs         # Panel de administración
├── Models/
│   ├── ApplicationUser.cs
│   ├── Curso.cs
│   ├── Leccion.cs
│   ├── Inscripcion.cs
│   ├── LeccionCompletada.cs
│   └── RolesConstantes.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   └── RegisterViewModel.cs
├── Services/           # Lógica de negocio (interfaces + implementaciones)
├── Repositories/       # Acceso a datos (patrón Repository + UnitOfWork)
├── Data/               # ApplicationDbContext + SeedData
├── Migrations/         # Migraciones EF Core
└── Views/
    ├── Home/           # Index, Privacy
    ├── Account/        # Login, Register, AccessDenied
    ├── Cursos/         # Index (catálogo), Detalle
    ├── Inscripciones/  # MisCursos, Confirmacion
    ├── Lecciones/      # Ver (contenido)
    ├── Progreso/       # Detalle (progreso por curso)
    ├── Admin/          # Panel, Cursos, CRUD, Lecciones, Suscriptores
    └── Shared/         # _Layout, Error, _ValidationScriptsPartial
```

## Funcionalidades por rol

### Invitado (no autenticado)
- Ver página de inicio con cursos destacados
- Explorar el catálogo con búsqueda y filtros por nivel
- Ver el detalle de un curso (descripción, lista de lecciones bloqueadas)
- Registrarse o iniciar sesión

### Suscriptor (usuario registrado)
- Todo lo anterior
- Inscribirse en cursos publicados
- Acceder a lecciones de cursos inscritos
- Marcar lecciones como completadas
- Ver barra de progreso por curso
- Cancelar inscripciones
- Ver progreso detallado por curso

### Administrador
- Todo lo anterior
- Panel de administración con estadísticas
- CRUD completo de cursos (crear, editar, eliminar)
- Publicar / despublicar cursos (requiere al menos 1 lección)
- CRUD completo de lecciones (con contenido HTML)
- Consultar suscriptores inscritos por curso
- Acceso a lecciones sin necesidad de inscripción

## Rutas principales

| Ruta                              | Descripción                         | Acceso           |
|-----------------------------------|-------------------------------------|------------------|
| `/`                               | Página principal                    | Público          |
| `/Cursos`                         | Catálogo de cursos                  | Público          |
| `/Cursos/Detalle/{id}`            | Detalle de un curso                 | Público          |
| `/Account/Login`                  | Iniciar sesión                      | Anónimo          |
| `/Account/Register`               | Crear cuenta                        | Anónimo          |
| `/Inscripciones/MisCursos`        | Mis cursos inscritos                | Suscriptor/Admin |
| `/Inscripciones/Inscribir`        | POST: inscribirse en un curso       | Suscriptor/Admin |
| `/Lecciones/Ver`                  | Ver contenido de una lección        | Suscriptor/Admin |
| `/Progreso/Detalle/{id}`          | Progreso detallado por curso        | Suscriptor/Admin |
| `/Admin`                          | Panel de administración             | Administrador    |
| `/Admin/Cursos`                   | Gestionar cursos                    | Administrador    |
| `/Admin/Suscriptores`             | Suscriptores por curso              | Administrador    |

## Reglas de negocio importantes

1. Un curso **no puede publicarse** si no tiene al menos una lección.
2. Un curso **no puede eliminarse** si tiene usuarios inscritos.
3. Un usuario **no puede inscribirse dos veces** en el mismo curso.
4. Solo usuarios con rol **Suscriptor o Administrador** pueden inscribirse.
5. Las lecciones solo son accesibles para usuarios **inscritos** en el curso (o Administradores).
6. Al registrarse, el usuario recibe automáticamente el rol **Suscriptor**.

## Scripts de datos de prueba

Los datos de prueba se cargan automáticamente al iniciar la aplicación mediante `SeedData.cs`.
Incluyen:

- **3 cursos** de ejemplo (2 publicados, 1 en borrador)
- Lecciones con contenido HTML en cada curso
- Usuario administrador (`admin@portalmicursos.com` / `Admin123`)

Para recrear los datos desde cero:
```bash
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

## Tecnologías utilizadas

- ASP.NET Core MVC 10.0
- Entity Framework Core 10.0 (SQL Server)
- ASP.NET Core Identity
- Bootstrap 5 + Bootstrap Icons
- jQuery Validation (validación cliente)
- Patrón Repository + Unit of Work
