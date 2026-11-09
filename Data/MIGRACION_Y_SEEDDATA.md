# Migración y Seed Data

## Base de Datos Creada

La migración **MigracionInicial** ha creado exitosamente la base de datos `PortalMicrocursosDB` con las siguientes tablas:

### Tablas de Identity (7 tablas)
1. **AspNetUsers** - Usuarios del sistema
2. **AspNetRoles** - Roles (Invitado, Suscriptor, Administrador)
3. **AspNetUserRoles** - Relación usuarios-roles
4. **AspNetUserClaims** - Claims de usuarios
5. **AspNetRoleClaims** - Claims de roles
6. **AspNetUserLogins** - Logins externos
7. **AspNetUserTokens** - Tokens de autenticación

### Tablas Personalizadas (4 tablas)
1. **Cursos** - Información de los cursos
2. **Lecciones** - Lecciones de cada curso
3. **Inscripciones** - Inscripciones de usuarios a cursos
4. **LeccionesCompletadas** - Progreso de usuarios

### Tabla de Control
- **__EFMigrationsHistory** - Historial de migraciones aplicadas

---

## Datos Iniciales (Seed Data)

### 1. Roles Creados
El sistema inicializa automáticamente tres roles:

- ✅ **Invitado** - Usuario no autenticado
- ✅ **Suscriptor** - Usuario registrado
- ✅ **Administrador** - Usuario con permisos completos

### 2. Usuario Administrador

Se crea un usuario administrador por defecto:

```
Email: admin@portalmicrocursos.com
Contraseña: Admin123
Rol: Administrador
```

⚠️ **IMPORTANTE:** Cambiar esta contraseña en producción por seguridad.

### 3. Cursos de Ejemplo

Se crean 3 cursos de ejemplo con sus lecciones:

#### Curso 1: Introducción a ASP.NET Core
- **Nivel:** Básico
- **Duración:** 180 minutos (3 horas)
- **Estado:** Publicado ✅
- **Lecciones:** 3 lecciones
  1. Bienvenida al curso
  2. Configuración del entorno de desarrollo
  3. Estructura de un proyecto MVC

#### Curso 2: Entity Framework Core - Base de Datos
- **Nivel:** Intermedio
- **Duración:** 240 minutos (4 horas)
- **Estado:** Publicado ✅
- **Lecciones:** 2 lecciones
  1. ¿Qué es Entity Framework Core?
  2. Code First y Migraciones

#### Curso 3: Seguridad con ASP.NET Core Identity
- **Nivel:** Avanzado
- **Duración:** 300 minutos (5 horas)
- **Estado:** NO Publicado ❌
- **Lecciones:** 1 lección
  1. Fundamentos de Identity

---

## Comandos Útiles de Migraciones

### Crear una nueva migración
```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Aplicar migraciones pendientes
```bash
dotnet ef database update
```

### Revertir a una migración específica
```bash
dotnet ef database update NombreMigracionAnterior
```

### Eliminar la última migración (si no se ha aplicado)
```bash
dotnet ef migrations remove
```

### Ver lista de migraciones
```bash
dotnet ef migrations list
```

### Generar script SQL de las migraciones
```bash
dotnet ef migrations script
```

### Eliminar la base de datos completa
```bash
dotnet ef database drop
```

---

## Verificar la Base de Datos

### Usando SQL Server Management Studio (SSMS)
1. Conectarse a: `(localdb)\mssqllocaldb`
2. Buscar base de datos: `PortalMicrocursosDB`
3. Explorar tablas y datos

### Usando Azure Data Studio
1. Conectarse a: `(localdb)\mssqllocaldb`
2. Ver base de datos: `PortalMicrocursosDB`

### Consultas SQL de Verificación

```sql
-- Ver todos los roles
SELECT * FROM AspNetRoles;

-- Ver todos los usuarios
SELECT Id, UserName, Email, NombreCompleto, FechaRegistro 
FROM AspNetUsers;

-- Ver usuarios y sus roles
SELECT u.Email, r.Name as Rol
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id;

-- Ver cursos publicados
SELECT Id, Titulo, Nivel, Duracion, EstaPublicado
FROM Cursos
WHERE EstaPublicado = 1;

-- Ver lecciones de un curso
SELECT l.Orden, l.Titulo, c.Titulo as Curso
FROM Lecciones l
INNER JOIN Cursos c ON l.CursoId = c.Id
ORDER BY c.Id, l.Orden;

-- Ver inscripciones
SELECT u.Email, c.Titulo as Curso, i.FechaInscripcion
FROM Inscripciones i
INNER JOIN AspNetUsers u ON i.UsuarioId = u.Id
INNER JOIN Cursos c ON i.CursoId = c.Id;
```

---

## Ejecución del Seed Data

El método `SeedData.Initialize()` se ejecuta automáticamente al iniciar la aplicación (ver `Program.cs`).

### Comportamiento:
- ✅ Se ejecuta en cada inicio de la aplicación
- ✅ Es **idempotente**: verifica si los datos ya existen antes de crearlos
- ✅ No duplica roles, usuarios o cursos
- ✅ Muestra mensajes en la consola indicando qué se creó

### Salida de Consola Esperada:
```
✓ Rol creado: Invitado
✓ Rol creado: Suscriptor
✓ Rol creado: Administrador
✓ Usuario administrador creado
  Email: admin@portalmicrocursos.com
  Password: Admin123
  ⚠️ IMPORTANTE: Cambiar la contraseña en producción
✓ Cursos y lecciones de ejemplo creados
```

---

## Deshabilitar Cursos de Ejemplo

Si deseas deshabilitar la creación de cursos de ejemplo, modifica el método `SeedData.Initialize()` comentando la línea:

```csharp
// await CrearCursosEjemplo(context); // Comentar para deshabilitar
```

---

## Agregar Más Datos de Ejemplo

Para agregar más cursos de ejemplo, edita el método `CrearCursosEjemplo()` en `Data/SeedData.cs`.

Ejemplo:
```csharp
var cursoNuevo = new Curso
{
    Titulo = "Tu Nuevo Curso",
    Descripcion = "Descripción del curso",
    Nivel = "Básico",
    Duracion = 120,
    EstaPublicado = true,
    FechaCreacion = DateTime.Now
};

context.Cursos.Add(cursoNuevo);
await context.SaveChangesAsync();
```

---

## Troubleshooting

### Error: "The database already exists"
La base de datos ya está creada. Puedes:
1. Usar `dotnet ef database drop` para eliminarla y recrearla
2. O simplemente ejecutar `dotnet ef database update` para aplicar nuevas migraciones

### Error: "Cannot insert duplicate key"
Los datos ya existen. El SeedData es idempotente, así que esto no debería ocurrir. Verifica que las validaciones en SeedData estén funcionando correctamente.

### Error: "The EF Core tools version is older"
Las herramientas de EF Core necesitan actualización:
```bash
dotnet tool update --global dotnet-ef
```

### No aparecen los datos de ejemplo
Verifica la consola de la aplicación para ver los mensajes del SeedData. Si hay errores, aparecerán ahí.

---

## Próximos Pasos (Fase 7)

Con la base de datos y datos iniciales listos, en la Fase 7 implementaremos los **Repositorios** para acceder a los datos de forma estructurada.
