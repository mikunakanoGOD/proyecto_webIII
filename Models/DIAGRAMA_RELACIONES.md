# Diagrama de Relaciones - Portal de Microcursos

## Modelo de Datos Completo

```
┌─────────────────────────┐
│   ApplicationUser       │
│   (Identity)            │
├─────────────────────────┤
│ Id (string, PK)         │
│ UserName                │
│ Email                   │
│ PasswordHash            │
│ NombreCompleto          │
│ FechaRegistro           │
└──────────┬──────────────┘
           │
           │ 1:N
           │
           ├──────────────────────────────┐
           │                              │
           │                              │
┌──────────▼──────────────┐    ┌─────────▼───────────────┐
│   Inscripcion           │    │  LeccionCompletada      │
├─────────────────────────┤    ├─────────────────────────┤
│ Id (int, PK)            │    │ Id (int, PK)            │
│ UsuarioId (string, FK)  │    │ UsuarioId (string, FK)  │
│ CursoId (int, FK)       │    │ LeccionId (int, FK)     │
│ FechaInscripcion        │    │ FechaCompletada         │
└──────────┬──────────────┘    └─────────┬───────────────┘
           │                             │
           │ N:1                         │ N:1
           │                             │
┌──────────▼──────────────┐    ┌─────────▼───────────────┐
│   Curso                 │    │   Leccion               │
├─────────────────────────┤    ├─────────────────────────┤
│ Id (int, PK)            │    │ Id (int, PK)            │
│ Titulo                  │    │ Titulo                  │
│ Descripcion             │    │ Contenido               │
│ Nivel                   │    │ Orden                   │
│ Duracion                │    │ CursoId (int, FK)       │
│ EstaPublicado           │    └─────────┬───────────────┘
│ FechaCreacion           │              │
└──────────┬──────────────┘              │
           │                             │
           │ 1:N                         │
           └─────────────────────────────┘
```

## Descripción de Relaciones

### 1. ApplicationUser → Inscripcion (1:N)
- Un usuario puede tener **muchas inscripciones** (a diferentes cursos)
- Una inscripción pertenece a **un solo usuario**
- Cascade Delete: Si se elimina un usuario, se eliminan sus inscripciones

### 2. ApplicationUser → LeccionCompletada (1:N)
- Un usuario puede tener **muchas lecciones completadas**
- Una lección completada pertenece a **un solo usuario**
- Cascade Delete: Si se elimina un usuario, se eliminan sus registros de progreso

### 3. Curso → Inscripcion (1:N)
- Un curso puede tener **muchas inscripciones** (de diferentes usuarios)
- Una inscripción pertenece a **un solo curso**
- Cascade Delete: Si se elimina un curso, se eliminan sus inscripciones

### 4. Curso → Leccion (1:N)
- Un curso puede tener **muchas lecciones**
- Una lección pertenece a **un solo curso**
- Cascade Delete: Si se elimina un curso, se eliminan sus lecciones

### 5. Leccion → LeccionCompletada (1:N)
- Una lección puede estar completada por **muchos usuarios**
- Una lección completada pertenece a **una sola lección**
- Cascade Delete: Si se elimina una lección, se eliminan sus registros de progreso

## Restricciones de Unicidad

### Inscripcion
```sql
UNIQUE INDEX: (UsuarioId, CursoId)
```
- Un usuario NO puede inscribirse dos veces en el mismo curso

### LeccionCompletada
```sql
UNIQUE INDEX: (UsuarioId, LeccionId)
```
- Un usuario NO puede marcar la misma lección como completada más de una vez

### Leccion
```sql
UNIQUE INDEX: (CursoId, Orden)
```
- Dentro de un curso, NO puede haber dos lecciones con el mismo número de orden

## Índices para Rendimiento

1. **Curso.Titulo** - Para búsquedas de cursos por nombre
2. **Curso.EstaPublicado** - Para filtrar cursos publicados
3. **Leccion.(CursoId, Orden)** - Para ordenar lecciones de un curso
4. **Inscripcion.(UsuarioId, CursoId)** - Para verificar inscripciones
5. **LeccionCompletada.(UsuarioId, LeccionId)** - Para verificar progreso

## Ejemplos de Consultas

### ¿Está un usuario inscrito en un curso?
```csharp
var inscrito = await _context.Inscripciones
    .AnyAsync(i => i.UsuarioId == usuarioId && i.CursoId == cursoId);
```

### Obtener todas las lecciones de un curso (ordenadas)
```csharp
var lecciones = await _context.Lecciones
    .Where(l => l.CursoId == cursoId)
    .OrderBy(l => l.Orden)
    .ToListAsync();
```

### Calcular progreso de un usuario en un curso
```csharp
var totalLecciones = await _context.Lecciones
    .CountAsync(l => l.CursoId == cursoId);
    
var leccionesCompletadas = await _context.LeccionesCompletadas
    .CountAsync(lc => lc.UsuarioId == usuarioId && 
                      lc.Leccion.CursoId == cursoId);
    
var progreso = (leccionesCompletadas * 100) / totalLecciones;
```

### Obtener cursos inscritos de un usuario con progreso
```csharp
var cursosInscritos = await _context.Inscripciones
    .Where(i => i.UsuarioId == usuarioId)
    .Include(i => i.Curso)
        .ThenInclude(c => c.Lecciones)
    .Select(i => new {
        Curso = i.Curso,
        TotalLecciones = i.Curso.Lecciones.Count,
        Completadas = i.Curso.Lecciones
            .Count(l => l.LeccionesCompletadas
                .Any(lc => lc.UsuarioId == usuarioId))
    })
    .ToListAsync();
```
