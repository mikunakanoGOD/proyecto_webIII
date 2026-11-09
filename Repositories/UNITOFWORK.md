# Unit of Work Pattern

## ¿Qué es el Patrón Unit of Work?

El patrón **Unit of Work** mantiene una lista de objetos afectados por una transacción de negocio y **coordina la escritura de cambios** en la base de datos.

### Beneficios:

- ✅ **Transacciones coordinadas**: Múltiples operaciones se ejecutan como una unidad atómica
- ✅ **Centralización**: Un solo punto para guardar cambios
- ✅ **Integridad de datos**: Todo se guarda o nada se guarda (rollback automático)
- ✅ **Reducción de llamadas a BD**: Se agrupa SaveChanges() en un solo punto
- ✅ **Acceso centralizado**: Un solo objeto para acceder a todos los repositorios

## Implementación en este Proyecto

### IUnitOfWork - Interfaz

```csharp
public interface IUnitOfWork : IDisposable
{
    ICursoRepository Cursos { get; }
    ILeccionRepository Lecciones { get; }
    IInscripcionRepository Inscripciones { get; }
    ILeccionCompletadaRepository LeccionesCompletadas { get; }
    
    Task<int> GuardarCambiosAsync();
    Task IniciarTransaccionAsync();
    Task ConfirmarTransaccionAsync();
    Task RevertirTransaccionAsync();
}
```

### UnitOfWork - Implementación

La clase `UnitOfWork` implementa:

1. **Lazy Loading de Repositorios**: Los repositorios se crean solo cuando se necesitan
2. **DbContext Compartido**: Todos los repositorios usan el mismo contexto
3. **Gestión de Transacciones**: Manejo completo de transacciones de base de datos
4. **Dispose Pattern**: Libera recursos correctamente

## Ejemplos de Uso

### 1. Uso Básico - Sin Transacción Explícita

```csharp
public class MiServicio
{
    private readonly IUnitOfWork _unitOfWork;

    public MiServicio(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CrearCursoConLecciones(Curso curso, List<Leccion> lecciones)
    {
        // Agregar curso
        await _unitOfWork.Cursos.AgregarAsync(curso);
        
        // Agregar lecciones
        foreach (var leccion in lecciones)
        {
            leccion.CursoId = curso.Id;
            await _unitOfWork.Lecciones.AgregarAsync(leccion);
        }
        
        // Todo se guarda en una sola operación
        await _unitOfWork.GuardarCambiosAsync();
    }
}
```

### 2. Uso con Transacción Explícita

Cuando necesitas garantizar que **todo se ejecute o nada**:

```csharp
public async Task InscribirUsuarioYMarcarPrimeraLeccion(string usuarioId, int cursoId)
{
    try
    {
        // Iniciar transacción
        await _unitOfWork.IniciarTransaccionAsync();

        // 1. Verificar si ya está inscrito
        var yaInscrito = await _unitOfWork.Inscripciones
            .VerificarInscripcionAsync(usuarioId, cursoId);
            
        if (yaInscrito)
            throw new Exception("Ya está inscrito en este curso");

        // 2. Crear inscripción
        var inscripcion = new Inscripcion
        {
            UsuarioId = usuarioId,
            CursoId = cursoId,
            FechaInscripcion = DateTime.Now
        };
        await _unitOfWork.Inscripciones.AgregarAsync(inscripcion);

        // 3. Obtener primera lección
        var primeraLeccion = await _unitOfWork.Lecciones
            .ObtenerPrimeraLeccionAsync(cursoId);

        // 4. Marcar como completada (opcional)
        if (primeraLeccion != null)
        {
            await _unitOfWork.LeccionesCompletadas
                .MarcarComoCompletadaAsync(usuarioId, primeraLeccion.Id);
        }

        // Confirmar transacción (todo se guarda aquí)
        await _unitOfWork.ConfirmarTransaccionAsync();
    }
    catch
    {
        // Si algo falla, revertir TODOS los cambios
        await _unitOfWork.RevertirTransaccionAsync();
        throw;
    }
}
```

### 3. Operaciones Complejas

```csharp
public async Task ActualizarCursoCompleto(
    Curso curso, 
    List<Leccion> leccionesNuevas, 
    List<int> leccionesAEliminar)
{
    try
    {
        await _unitOfWork.IniciarTransaccionAsync();

        // 1. Actualizar curso
        await _unitOfWork.Cursos.ActualizarAsync(curso);

        // 2. Eliminar lecciones
        foreach (var leccionId in leccionesAEliminar)
        {
            await _unitOfWork.Lecciones.EliminarAsync(leccionId);
        }

        // 3. Agregar nuevas lecciones
        foreach (var leccion in leccionesNuevas)
        {
            leccion.CursoId = curso.Id;
            await _unitOfWork.Lecciones.AgregarAsync(leccion);
        }

        // Confirmar todo de una vez
        await _unitOfWork.ConfirmarTransaccionAsync();
    }
    catch
    {
        await _unitOfWork.RevertirTransaccionAsync();
        throw;
    }
}
```

### 4. Consultas Relacionadas

```csharp
public async Task<ProgresoCursoViewModel> ObtenerProgresoDetallado(string usuarioId, int cursoId)
{
    // Verificar inscripción
    var inscrito = await _unitOfWork.Inscripciones
        .VerificarInscripcionAsync(usuarioId, cursoId);
        
    if (!inscrito)
        return null;

    // Obtener curso con lecciones
    var curso = await _unitOfWork.Cursos
        .ObtenerCursoConLeccionesAsync(cursoId);

    // Obtener IDs de lecciones completadas
    var leccionesCompletadas = await _unitOfWork.LeccionesCompletadas
        .ObtenerIdsLeccionesCompletadasAsync(usuarioId, cursoId);

    // Calcular progreso
    var progreso = await _unitOfWork.LeccionesCompletadas
        .CalcularProgresoAsync(usuarioId, cursoId);

    return new ProgresoCursoViewModel
    {
        Curso = curso,
        LeccionesCompletadas = leccionesCompletadas,
        PorcentajeProgreso = progreso
    };
}
```

## Cuándo Usar Unit of Work vs Repositorios Directos

### Usa Unit of Work cuando:
- ✅ Necesitas **coordinar múltiples repositorios** en una operación
- ✅ Requieres **transacciones explícitas** (todo o nada)
- ✅ Quieres **centralizar el control** de guardado de cambios
- ✅ Trabajas en **operaciones de negocio complejas**

### Usa Repositorios Directos cuando:
- ✅ Solo necesitas **un repositorio**
- ✅ Son **operaciones simples** de lectura o escritura
- ✅ No requieres transacciones coordinadas

## Registro en Program.cs

```csharp
// Registrar repositorios individuales (Scoped)
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ILeccionRepository, LeccionRepository>();
builder.Services.AddScoped<IInscripcionRepository, InscripcionRepository>();
builder.Services.AddScoped<ILeccionCompletadaRepository, LeccionCompletadaRepository>();

// Registrar Unit of Work (Scoped)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

**Importante:** Se usa `AddScoped` porque queremos una instancia por request HTTP.

## Ventajas de Nuestra Implementación

### 1. Lazy Loading de Repositorios
Los repositorios se crean solo cuando se acceden:
```csharp
public ICursoRepository Cursos
{
    get
    {
        _cursoRepository ??= new CursoRepository(_context);
        return _cursoRepository;
    }
}
```

### 2. DbContext Compartido
Todos los repositorios comparten el mismo `ApplicationDbContext`, lo que permite:
- Tracking de cambios unificado
- Transacciones coordinadas
- Optimización de consultas

### 3. Manejo Robusto de Transacciones
```csharp
public async Task ConfirmarTransaccionAsync()
{
    try
    {
        await GuardarCambiosAsync();
        await _transaction.CommitAsync();
    }
    catch
    {
        await RevertirTransaccionAsync();
        throw;
    }
    finally
    {
        await _transaction.DisposeAsync();
    }
}
```

## Antipatrones a Evitar

### ❌ No Guardar Cambios
```csharp
// MALO: Los cambios nunca se guardan
await _unitOfWork.Cursos.AgregarAsync(curso);
// Falta: await _unitOfWork.GuardarCambiosAsync();
```

### ❌ Transacción Sin Try-Catch
```csharp
// MALO: Si falla, la transacción queda abierta
await _unitOfWork.IniciarTransaccionAsync();
await _unitOfWork.Cursos.AgregarAsync(curso);
await _unitOfWork.ConfirmarTransaccionAsync();
// Falta: try-catch para RevertirTransaccionAsync()
```

### ❌ No Disponer del UnitOfWork
```csharp
// MALO: Memory leak
var unitOfWork = new UnitOfWork(context);
await unitOfWork.Cursos.AgregarAsync(curso);
// Falta: unitOfWork.Dispose() o using statement
```

## Buenas Prácticas

### ✅ Usar Using Statement
```csharp
using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
await unitOfWork.Cursos.AgregarAsync(curso);
await unitOfWork.GuardarCambiosAsync();
// Se dispone automáticamente
```

### ✅ Inyección de Dependencias
```csharp
public class MiControlador : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    
    public MiControlador(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    // El framework se encarga del ciclo de vida
}
```

### ✅ Transacciones Explícitas Solo Cuando Sea Necesario
```csharp
// Para operaciones simples, SaveChangesAsync es suficiente
await _unitOfWork.Cursos.AgregarAsync(curso);
await _unitOfWork.GuardarCambiosAsync();

// Para operaciones complejas, usa transacciones explícitas
await _unitOfWork.IniciarTransaccionAsync();
// ... múltiples operaciones
await _unitOfWork.ConfirmarTransaccionAsync();
```

## Próximos Pasos (Fase 9)

Con los repositorios y UnitOfWork listos, en la Fase 9 implementaremos la **capa de servicios** que usará estos repositorios para implementar la lógica de negocio.

## Referencias

- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [EF Core Transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions)
