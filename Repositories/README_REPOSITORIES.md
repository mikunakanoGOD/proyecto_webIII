# Repositorios - Patrón Repository

## ¿Qué es el Patrón Repository?

El patrón Repository actúa como una **capa de abstracción** entre la lógica de negocio y el acceso a datos. Sus beneficios incluyen:

- ✅ **Separación de responsabilidades**: La lógica de negocio no conoce los detalles de cómo se accede a los datos
- ✅ **Testabilidad**: Fácil de crear mocks para pruebas unitarias
- ✅ **Mantenibilidad**: Cambios en el acceso a datos no afectan la lógica de negocio
- ✅ **Reutilización**: Consultas comunes se definen una vez y se reusan
- ✅ **Consistencia**: Patrones de acceso a datos uniformes en toda la aplicación

## Estructura de Repositorios

### 1. IRepository<T> - Interfaz Genérica
Define operaciones CRUD básicas para cualquier entidad:
- `ObtenerTodosAsync()` - Obtener todas las entidades
- `ObtenerPorIdAsync(id)` - Obtener por ID
- `AgregarAsync(entidad)` - Crear nueva entidad
- `ActualizarAsync(entidad)` - Actualizar entidad existente
- `EliminarAsync(id)` - Eliminar por ID
- `ExisteAsync(id)` - Verificar existencia

### 2. Repository<T> - Implementación Genérica
Implementa las operaciones CRUD básicas usando Entity Framework Core.
Otras clases de repositorio heredan de esta clase base.

### 3. Repositorios Específicos
Cada entidad principal tiene su interfaz e implementación específica:

#### ICursoRepository / CursoRepository
**Operaciones adicionales:**
- `ObtenerCursosPublicadosAsync()` - Solo cursos publicados
- `ObtenerCursoConLeccionesAsync(id)` - Incluye lecciones relacionadas
- `BuscarPorTituloAsync(titulo)` - Búsqueda por título
- `ObtenerPorNivelAsync(nivel)` - Filtrar por nivel (Básico, Intermedio, Avanzado)
- `PublicarCursoAsync(id)` - Cambiar estado a publicado
- `DespublicarCursoAsync(id)` - Cambiar estado a no publicado

#### ILeccionRepository / LeccionRepository
**Operaciones adicionales:**
- `ObtenerPorCursoAsync(cursoId)` - Todas las lecciones de un curso (ordenadas)
- `ObtenerLeccionDeCursoAsync(cursoId, leccionId)` - Lección específica
- `ObtenerPrimeraLeccionAsync(cursoId)` - Primera lección del curso
- `ObtenerSiguienteLeccionAsync(cursoId, orden)` - Navegación siguiente
- `ObtenerLeccionAnteriorAsync(cursoId, orden)` - Navegación anterior
- `ContarLeccionesDeCursoAsync(cursoId)` - Cantidad de lecciones
- `ReordenarLeccionesAsync(cursoId, ordenes)` - Cambiar orden de lecciones

## Ejemplos de Uso

### En un Servicio o Controlador

```csharp
public class CursoService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ILeccionRepository _leccionRepository;

    public CursoService(
        ICursoRepository cursoRepository,
        ILeccionRepository leccionRepository)
    {
        _cursoRepository = cursoRepository;
        _leccionRepository = leccionRepository;
    }

    // Obtener cursos publicados
    public async Task<IEnumerable<Curso>> ObtenerCursosDisponibles()
    {
        return await _cursoRepository.ObtenerCursosPublicadosAsync();
    }

    // Obtener curso con sus lecciones
    public async Task<Curso?> ObtenerCursoCompleto(int id)
    {
        return await _cursoRepository.ObtenerCursoConLeccionesAsync(id);
    }

    // Crear nuevo curso
    public async Task<Curso> CrearCurso(Curso curso)
    {
        return await _cursoRepository.AgregarAsync(curso);
    }

    // Publicar curso
    public async Task PublicarCurso(int id)
    {
        await _cursoRepository.PublicarCursoAsync(id);
    }
}
```

### Navegación de Lecciones

```csharp
// Obtener primera lección de un curso
var primeraLeccion = await _leccionRepository.ObtenerPrimeraLeccionAsync(cursoId);

// Navegar a la siguiente lección
var siguienteLeccion = await _leccionRepository
    .ObtenerSiguienteLeccionAsync(cursoId, leccionActual.Orden);

// Navegar a la lección anterior
var leccionAnterior = await _leccionRepository
    .ObtenerLeccionAnteriorAsync(cursoId, leccionActual.Orden);

// Contar total de lecciones
var totalLecciones = await _leccionRepository.ContarLeccionesDeCursoAsync(cursoId);
```

### Búsqueda de Cursos

```csharp
// Buscar por título
var cursos = await _cursoRepository.BuscarPorTituloAsync("ASP.NET");

// Filtrar por nivel
var cursosBasicos = await _cursoRepository.ObtenerPorNivelAsync("Básico");

// Verificar si existe un curso
var existe = await _cursoRepository.ExisteAsync(cursoId);
```

## Registro de Repositorios en Program.cs

Los repositorios deben registrarse en el contenedor de inyección de dependencias:

```csharp
// Registrar repositorios
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ILeccionRepository, LeccionRepository>();
// ... otros repositorios
```

**Nota:** Esto se hará en la Fase 8 junto con los demás repositorios.

## Beneficios de este Enfoque

### 1. Testabilidad
```csharp
// En tests unitarios, podemos crear mocks fácilmente
var mockCursoRepo = new Mock<ICursoRepository>();
mockCursoRepo.Setup(r => r.ObtenerCursosPublicadosAsync())
    .ReturnsAsync(cursosDePrueba);

var service = new CursoService(mockCursoRepo.Object);
```

### 2. Cambio de Implementación
Si en el futuro necesitamos cambiar de Entity Framework a otro ORM, solo modificamos la implementación del repositorio, no toda la aplicación.

### 3. Operaciones Complejas Encapsuladas
```csharp
// En lugar de escribir esto en múltiples lugares:
var cursos = await context.Cursos
    .Where(c => c.EstaPublicado)
    .OrderByDescending(c => c.FechaCreacion)
    .ToListAsync();

// Simplemente llamamos:
var cursos = await _cursoRepository.ObtenerCursosPublicadosAsync();
```

## Próximos Repositorios (Fase 8)

En la siguiente fase implementaremos:
- `IInscripcionRepository / InscripcionRepository`
- `ILeccionCompletadaRepository / LeccionCompletadaRepository`
- `IUnitOfWork / UnitOfWork` - Para coordinar transacciones

## Notas Importantes

### Async/Await
Todas las operaciones son **asíncronas** para no bloquear el hilo de ejecución, especialmente importante en aplicaciones web.

### Include() para Relaciones
Cuando necesitamos cargar entidades relacionadas, usamos `.Include()`:
```csharp
// Cargar curso con sus lecciones
.Include(c => c.Lecciones)

// Cargar múltiples relaciones
.Include(c => c.Lecciones)
.Include(c => c.Inscripciones)
```

### Virtual en las Propiedades de Navegación
Las propiedades de navegación en los modelos son `virtual` para permitir **lazy loading** (carga diferida), aunque en este proyecto usamos **eager loading** explícito con `.Include()`.

### ToListAsync() vs FirstOrDefaultAsync()
- `ToListAsync()` - Para obtener múltiples registros
- `FirstOrDefaultAsync()` - Para obtener un solo registro (puede ser null)
- `SingleOrDefaultAsync()` - Para obtener exactamente uno o ninguno (lanza excepción si hay más de uno)

## Referencias

- [Entity Framework Core Queries](https://learn.microsoft.com/en-us/ef/core/querying/)
- [Repository Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
