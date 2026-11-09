# Configuración de ASP.NET Core Identity

## Descripción General

Este proyecto utiliza **ASP.NET Core Identity** para gestionar la autenticación y autorización de usuarios.

## Tablas de Identity Creadas Automáticamente

Al heredar de `IdentityDbContext<ApplicationUser>`, Entity Framework crea automáticamente las siguientes tablas:

### 1. AspNetUsers
- Almacena los usuarios del sistema
- Campos principales:
  - `Id` (string, PK) - Identificador único del usuario
  - `UserName` - Nombre de usuario
  - `Email` - Correo electrónico
  - `PasswordHash` - Contraseña hasheada (nunca en texto plano)
  - `PhoneNumber` - Teléfono
  - `EmailConfirmed` - Si el email está confirmado
  - `LockoutEnd` - Fecha de bloqueo de cuenta
  - `AccessFailedCount` - Intentos fallidos de login
  - **Campos personalizados:**
    - `NombreCompleto` - Nombre completo del usuario
    - `FechaRegistro` - Fecha de registro en el sistema

### 2. AspNetRoles
- Almacena los roles del sistema
- Campos:
  - `Id` (string, PK)
  - `Name` - Nombre del rol (Invitado, Suscriptor, Administrador)
  - `NormalizedName` - Nombre normalizado para búsquedas

### 3. AspNetUserRoles
- Tabla de relación muchos a muchos entre usuarios y roles
- Campos:
  - `UserId` (string, FK a AspNetUsers)
  - `RoleId` (string, FK a AspNetRoles)

### 4. AspNetUserClaims
- Claims adicionales por usuario (permisos específicos)

### 5. AspNetRoleClaims
- Claims adicionales por rol

### 6. AspNetUserLogins
- Información de logins externos (Google, Facebook, etc.)

### 7. AspNetUserTokens
- Tokens de autenticación y reset de contraseña

## Configuración de Seguridad

### Políticas de Contraseñas
```csharp
options.Password.RequireDigit = true;              // Requiere al menos un dígito (0-9)
options.Password.RequireLowercase = true;          // Requiere minúsculas
options.Password.RequireUppercase = true;          // Requiere mayúsculas
options.Password.RequireNonAlphanumeric = false;   // NO requiere caracteres especiales
options.Password.RequiredLength = 6;               // Mínimo 6 caracteres
options.Password.RequiredUniqueChars = 1;          // Al menos 1 carácter único
```

**Ejemplo de contraseña válida:** `Usuario123`

### Bloqueo de Cuenta (Lockout)
```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Bloqueo por 5 minutos
options.Lockout.MaxFailedAccessAttempts = 5;                        // Después de 5 intentos fallidos
options.Lockout.AllowedForNewUsers = true;                          // Aplica a usuarios nuevos
```

### Configuración de Usuario
```csharp
options.User.RequireUniqueEmail = true;  // El email debe ser único
```

### Configuración de Cookies
```csharp
options.Cookie.HttpOnly = true;                    // Cookie solo accesible por HTTP (no JavaScript)
options.ExpireTimeSpan = TimeSpan.FromHours(2);    // Cookie expira en 2 horas
options.LoginPath = "/Account/Login";              // Ruta de login
options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta de acceso denegado
options.SlidingExpiration = true;                  // Renovar cookie automáticamente
```

## Roles del Sistema

Definidos en `Models/RolesConstantes.cs`:

### 1. Invitado
- Usuario no autenticado o sin rol específico
- Permisos mínimos: Ver catálogo de cursos

### 2. Suscriptor
- Usuario autenticado registrado
- Puede inscribirse en cursos y acceder a lecciones

### 3. Administrador
- Permisos completos
- Gestión de cursos y lecciones

## Flujo de Autenticación

### Registro de Usuario
1. Usuario completa formulario de registro
2. Se crea registro en `AspNetUsers`
3. Contraseña se hashea automáticamente (NO se guarda en texto plano)
4. Se asigna rol "Suscriptor" por defecto
5. Se crea registro en `AspNetUserRoles`

### Inicio de Sesión
1. Usuario ingresa email y contraseña
2. Identity verifica el hash de la contraseña
3. Si es correcta, crea cookie de autenticación
4. Usuario queda autenticado por 2 horas (configurable)

### Verificación de Roles
```csharp
// En un controlador
[Authorize(Roles = "Administrador")]
public IActionResult PanelAdmin()
{
    // Solo accesible por administradores
}

[Authorize(Roles = "Suscriptor,Administrador")]
public IActionResult MisCursos()
{
    // Accesible por suscriptores y administradores
}
```

## Uso en Controladores

### Obtener Usuario Actual
```csharp
// En un controlador que hereda de Controller
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
var userName = User.Identity.Name;
var isAuthenticated = User.Identity.IsAuthenticated;
```

### Verificar Roles
```csharp
if (User.IsInRole("Administrador"))
{
    // Lógica específica para administradores
}
```

### Usando UserManager y SignInManager
```csharp
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Crear usuario
    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await _userManager.CreateAsync(user, password);

    // Asignar rol
    await _userManager.AddToRoleAsync(user, RolesConstantes.Suscriptor);

    // Iniciar sesión
    await _signInManager.SignInAsync(user, isPersistent: false);

    // Cerrar sesión
    await _signInManager.SignOutAsync();
}
```

## Migración y Seed Data

En la **Fase 6** crearemos:
1. Migración inicial que genera todas las tablas
2. SeedData que inicializa:
   - Los tres roles (Invitado, Suscriptor, Administrador)
   - Un usuario administrador por defecto
   - Cursos y lecciones de ejemplo (opcional)

## Seguridad Importante

### ✅ Buenas Prácticas Implementadas:
- Contraseñas hasheadas automáticamente (nunca texto plano)
- Cookies HttpOnly (protección contra XSS)
- Bloqueo de cuenta después de intentos fallidos
- Email único por usuario
- Expiración de sesión (2 horas)

### ⚠️ Consideraciones para Producción:
- Cambiar `RequireConfirmedEmail = true` (requiere confirmación por email)
- Implementar confirmación de email con tokens
- Usar HTTPS en todas las comunicaciones
- Considerar autenticación de dos factores (2FA)
- Registrar intentos de acceso sospechosos

## Referencias

- [Documentación oficial ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Configuración de Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration)
