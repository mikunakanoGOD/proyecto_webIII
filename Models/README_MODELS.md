# Modelos de Datos - Portal de Microcursos

## Estructura de Identity

El proyecto utiliza **ASP.NET Core Identity** para la gestión de usuarios y roles.

### Tablas que Identity creará automáticamente:

1. **AspNetUsers** - Usuarios del sistema
   - Campos base: Id, UserName, Email, PasswordHash, PhoneNumber, etc.
   - Campos personalizados: NombreCompleto, FechaRegistro (ver ApplicationUser.cs)

2. **AspNetRoles** - Roles del sistema
   - Id, Name, NormalizedName
   - Roles definidos: Invitado, Suscriptor, Administrador

3. **AspNetUserRoles** - Relación muchos a muchos entre usuarios y roles
   - UserId, RoleId

4. **AspNetUserClaims** - Claims adicionales por usuario
5. **AspNetRoleClaims** - Claims adicionales por rol
6. **AspNetUserLogins** - Logins externos (Google, Facebook, etc.)
7. **AspNetUserTokens** - Tokens de autenticación

## Modelos Personalizados

Los siguientes modelos se crearán en la Fase 4:

### Curso
- Id (int, PK)
- Titulo (string)
- Descripcion (string)
- Nivel (string) - Ejemplo: Básico, Intermedio, Avanzado
- Duracion (int) - En minutos
- EstaPublicado (bool)
- FechaCreacion (DateTime)

### Lección
- Id (int, PK)
- Titulo (string)
- Contenido (string/HTML)
- Orden (int)
- CursoId (int, FK)

### Inscripción
- Id (int, PK)
- UsuarioId (string, FK a AspNetUsers)
- CursoId (int, FK)
- FechaInscripcion (DateTime)

### LecciónCompletada
- Id (int, PK)
- UsuarioId (string, FK a AspNetUsers)
- LeccionId (int, FK)
- FechaCompletada (DateTime)

## Relaciones

```
ApplicationUser (1) ----< (N) Inscripcion (N) >---- (1) Curso
ApplicationUser (1) ----< (N) LeccionCompletada (N) >---- (1) Leccion
Curso (1) ----< (N) Leccion
```

## Flujo de autorización

1. **Usuario se registra** → Se crea en AspNetUsers
2. **Sistema asigna rol "Suscriptor"** → Se crea registro en AspNetUserRoles
3. **Suscriptor se inscribe en curso** → Se crea Inscripcion
4. **Suscriptor completa lección** → Se crea LeccionCompletada
5. **Sistema valida acceso** → Verifica Inscripcion antes de mostrar Leccion

## Notas de Seguridad

- Las contraseñas se almacenan hasheadas por Identity (PasswordHash)
- Los roles se verifican mediante atributos [Authorize(Roles = "...")]
- El acceso a lecciones requiere verificación de inscripción en el servidor
- No se debe confiar en la URL para proteger contenido (validar en backend)
