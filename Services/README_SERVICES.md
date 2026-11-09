# Servicios - Capa de Lógica de Negocio

## ¿Qué es la Capa de Servicios?

La capa de servicios contiene la **lógica de negocio** de la aplicación. Se sitúa entre los controladores y los repositorios, proporcionando:

- ✅ **Validaciones de negocio**: Reglas que deben cumplirse antes de modificar datos
- ✅ **Orquestación**: Coordina múltiples repositorios para operaciones complejas
- ✅ **Logging**: Registro de operaciones importantes
- ✅ **Manejo de errores**: Captura y procesa excepciones de forma centralizada
- ✅ **Abstracción**: Los controladores no conocen los detalles de acceso a datos

## Arquitectura en Capas

```
┌─────────────────────────┐
│   Controladores (MVC)   │  ← Recibe requests del usuario
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│   Servicios (Negocio)   │  ← Valida y coordina operaciones
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│  Repositorios (Datos)   │  ← Accede a la base de datos
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│  Entity Framework Core  │
└─────────────────────────┘
```

## Servicios Implementados

### 1. ICursoService / CursoService

**Responsabilidades:**
- Gestión completa de cursos
- Validaciones antes de crear/actualizar
- Publicación/despublicación con validaciones
- Búsqueda y filtrado

**Métodos principales:**

#### Consulta
- `ObtenerCursosPublicadosAsync()` - Solo cursos visibles al público
- `ObtenerTodosCursosAsync()` - Todos (para administradores)
- `ObtenerCursoPorIdAsync(id)` - Un curso específico
- `ObtenerCursoConLeccionesAsync(id)` - Curso con lecciones incluidas
- `BuscarCursosPorTituloAsync(titulo)` - Búsqueda por texto
- `ObtenerCursosPorNivelAsync(nivel)` - Filtro por nivel

#### Modificación
- `CrearCursoAsync(curso)` - Crea curso con validaciones
- `ActualizarCursoAsync(curso)` - Actualiza con validaciones
- `EliminarCursoAsync(id)` - Elimina si no tiene inscripciones
- `PublicarCursoAsync(id)` - Valida que tenga lecciones antes de publicar
- `DespublicarCursoAsync(id)` - Oculta curso del público

#### Validaciones
- `ExisteCursoAsync(id)` - Verifica existencia
- `PuedeEliminarCursoAsync(id)` - Verifica si tiene inscripciones

### 2. ILeccionService / LeccionService

**Responsabilidades:**
- Gestión de lecciones de cursos
- Navegación entre lecciones
- Reordenamiento de lecciones
- Validaciones de coherencia

**Métodos principales:**

#### Consulta
- `ObtenerLeccionesPorCursoAsync(cursoId)` - Todas las lecciones ordenadas
- `ObtenerLeccionPorIdAsync(id)` - Una lección específica
- `ObtenerLeccionDeCursoAsync(cursoId, leccionId)` - Validando pertenencia
- `ObtenerPrimeraLeccionAsync(cursoId)` - Punto de inicio del curso
- `ObtenerSiguienteLeccionAsync(cursoId, orden)` - Navegación adelante
- `ObtenerLeccionAnteriorAsync(cursoId, orden)` - Navegación atrás
- `ContarLeccionesAsync(cursoId)` - Cantidad total

#### Modificación
- `CrearLeccionAsync(leccion)` - Crea con validaciones y auto-orden
- `ActualizarLeccionAsync(leccion)` - Actualiza con validaciones
- `EliminarLeccionAsync(id)` - Elimina lección
- `ReordenarLeccionesAsync(cursoId, ordenes)` - Cambia orden de lecciones

#### Utilidades
- `ExisteLeccionAsync(id)` - Verifica existencia
- `ObtenerSiguienteOrdenAsync(cursoId)` - Calcula orden para nueva lección

## Ejemplos de Uso

### En un Controlador

```csharp
public class CursosController : Controller
{
    private readonly ICursoService _cursoService;
    private readonly ILeccionService _leccionService;

    public CursosController(ICursoService cursoService, ILeccionService leccionService)
    {
        _cursoService = cursoService;
        _leccionService = leccionService;
    }

    // GET: Cursos
    public async Task<IActionResult> Index()
    {
        var cursos = await _cursoService.ObtenerCursosPublicadosAsync();
        return View(cursos);
    }

    // GET: Cursos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var curso = await _cursoService.ObtenerCursoConLeccionesAsync(id);
        if (curso == null)
            return NotFound();

        return View(curso);
    }

    // POST: Cursos/Create
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(Curso curso)
    {
        if (!ModelState.IsValid)
            return View(curso);

        try
        {
            await _cursoService.CrearCursoAsync(curso);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(curso);
        }
    }
}
```

### Operación Compleja con Múltiples Servicios

```csharp
public async Task<IActionResult> PublicarCurso(int id)
{
    try
    {
        // El servicio valida que tenga lecciones antes de publicar
        await _cursoService.PublicarCursoAsync(id);
        
        TempData["Success"] = "Curso publicado exitosamente";
        return RedirectToAction("Details", new { id });
    }
    catch (InvalidOperationException ex)
    {
        // Captura errores de validación de negocio
        TempData["Error"] = ex.Message;
        return RedirectToAction("Edit", new { id });
    }
}
```

### Navegación de Lecciones

```csharp
public async Task<IActionResult> VerLeccion(int cursoId, int leccionId)
{
    var leccion = await _leccionService.ObtenerLeccionDeCursoAsync(cursoId, leccionId);
    if (leccion == null)
        return NotFound();

    // Obtener lección anterior y siguiente para navegación
    var siguiente = await _leccionService.ObtenerSiguienteLeccionAsync(cursoId, leccion.Orden);
    var anterior = await _leccionService.ObtenerLeccionAnteriorAsync(cursoId, leccion.Orden);

    var viewModel = new LeccionViewModel
    {
        Leccion = leccion,
        LeccionSiguiente = siguiente,
        LeccionAnterior = anterior
    };

    return View(viewModel);
}
```

## Validaciones Implementadas

### CursoService

#### Crear/Actualizar Curso
- ✅ Título no puede estar vacío
- ✅ Descripción no puede estar vacía
- ✅ Duración debe ser mayor a cero

#### Publicar Curso
- ✅ El curso debe existir
- ✅ Debe tener al menos una lección

#### Eliminar Curso
- ✅ No puede tener inscripciones activas

### LeccionService

#### Crear/Actualizar Lección
- ✅ Título no puede estar vacío
- ✅ Contenido no puede estar vacío
- ✅ CursoId debe ser válido
- ✅ El curso debe existir
- ✅ Orden debe ser mayor a cero (al actualizar)

#### Reordenar Lecciones
- ✅ Todas las lecciones deben pertenecer al curso especificado

## Manejo de Errores

Los servicios capturan y logean errores, luego los propagan:

```csharp
public async Task<Curso> CrearCursoAsync(Curso curso)
{
    try
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(curso.Titulo))
            throw new ArgumentException("El título es obligatorio");

        // Operación
        return await _unitOfWork.Cursos.AgregarAsync(curso);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear curso: {CursoTitulo}", curso.Titulo);
        throw; // Re-lanza la excepción para que el controlador la maneje
    }
}
```

## Logging

Cada servicio registra operaciones importantes:

- ℹ️ **Information**: Operaciones exitosas (crear, actualizar, eliminar)
- ⚠️ **Warning**: Situaciones inusuales pero controladas
- ❌ **Error**: Excepciones y errores de negocio

Ejemplo de logs:
```
[Information] Curso creado: 5 - Introducción a ASP.NET Core
[Information] Curso publicado: 5 - Introducción a ASP.NET Core
[Error] Error al crear curso: Título duplicado
```

## Registro en Program.cs

```csharp
// Registrar servicios de negocio
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<ILeccionService, LeccionService>();
```

**Importante:** Se usa `AddScoped` para que cada request HTTP tenga su propia instancia.

## Beneficios de esta Arquitectura

### 1. Controladores Delgados
```csharp
// ANTES (sin servicios) - Controlador inflado
public async Task<IActionResult> Create(Curso curso)
{
    if (string.IsNullOrWhiteSpace(curso.Titulo))
        ModelState.AddModelError("Titulo", "Obligatorio");
    
    curso.FechaCreacion = DateTime.Now;
    _context.Cursos.Add(curso);
    await _context.SaveChangesAsync();
    // ... más lógica
}

// AHORA (con servicios) - Controlador delgado
public async Task<IActionResult> Create(Curso curso)
{
    await _cursoService.CrearCursoAsync(curso);
    return RedirectToAction(nameof(Index));
}
```

### 2. Reutilización
La misma validación se usa en:
- Controladores web
- APIs
- Tareas en segundo plano
- Tests unitarios

### 3. Testabilidad
```csharp
// Mock del servicio para tests
var mockCursoService = new Mock<ICursoService>();
mockCursoService.Setup(s => s.ObtenerCursosPublicadosAsync())
    .ReturnsAsync(cursosDePrueba);

var controller = new CursosController(mockCursoService.Object);
```

## Próximos Servicios (Fase 10)

En la siguiente fase implementaremos:
- `IInscripcionService / InscripcionService`
- `IProgresoService / ProgresoService`

Estos completarán la lógica de negocio del sistema.

## Buenas Prácticas Aplicadas

### ✅ Inyección de Dependencias
```csharp
public class CursoService : ICursoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CursoService> _logger;

    public CursoService(IUnitOfWork unitOfWork, ILogger<CursoService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
}
```

### ✅ Validaciones Explícitas
```csharp
if (string.IsNullOrWhiteSpace(curso.Titulo))
    throw new ArgumentException("El título del curso es obligatorio");
```

### ✅ Mensajes de Error Descriptivos
```csharp
throw new InvalidOperationException("No se puede publicar un curso sin lecciones");
```

### ✅ Logging Contextualizado
```csharp
_logger.LogInformation("Curso creado: {CursoId} - {CursoTitulo}", curso.Id, curso.Titulo);
```

## Referencias

- [Service Layer Pattern](https://martinfowler.com/eaaCatalog/serviceLayer.html)
- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
