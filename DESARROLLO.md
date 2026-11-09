# Plan de Desarrollo - Backend (Edwin)

Este documento describe las 10 fases de desarrollo que corresponden a los commits 1-10 del proyecto.

---

## ✅ FASE 1 (Commit 1): Inicializar proyecto y configuración base

### Tareas completadas:
- [x] Crear proyecto ASP.NET Core MVC
- [x] Configurar appsettings.json con connection string
- [x] Crear estructura de carpetas (Data, Services, Repositories, ViewModels)
- [x] Crear .gitignore
- [x] Crear README.md con información del proyecto

### Archivos creados/modificados:
- README.md
- appsettings.json
- .gitignore
- Estructura de carpetas base

### Comando para commit:
```bash
git add .
git commit -m "Commit 1: Inicializar proyecto y configuración base"
```

---

## 📋 FASE 2 (Commit 2): Configurar Entity Framework y SQL Server

### Tareas a realizar:
- [ ] Instalar paquetes NuGet:
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
  - Microsoft.EntityFrameworkCore.Design
- [ ] Crear clase ApplicationDbContext en carpeta Data
- [ ] Configurar DbContext en Program.cs
- [ ] Verificar connection string

### Archivos a crear/modificar:
- Data/ApplicationDbContext.cs
- Program.cs
- PortalMicrocursos.csproj (automático al instalar paquetes)

### Comando para commit:
```bash
git add .
git commit -m "Commit 2: Configurar Entity Framework y SQL Server"
```

---

## 📋 FASE 3 (Commit 3): Implementar modelos de datos (Parte 1)

### Tareas a realizar:
- [ ] Crear modelo Usuario (si se usa personalizado) o preparar para Identity
- [ ] Crear modelo Rol (si se usa personalizado) o preparar para Identity
- [ ] Documentar estructura de Identity

### Archivos a crear:
- Models/Usuario.cs (opcional, según implementación)
- Models/Rol.cs (opcional, según implementación)

### Comando para commit:
```bash
git add .
git commit -m "Commit 3: Implementar modelos de datos - Usuario y Rol"
```

---

## 📋 FASE 4 (Commit 4): Implementar modelos de datos (Parte 2)

### Tareas a realizar:
- [ ] Crear modelo Curso con propiedades:
  - Id, Titulo, Descripcion, Nivel, Duracion, EstaPublicado, FechaCreacion
- [ ] Crear modelo Leccion con propiedades:
  - Id, Titulo, Contenido, Orden, CursoId (FK)
- [ ] Crear modelo Inscripcion con propiedades:
  - Id, UsuarioId (FK), CursoId (FK), FechaInscripcion
- [ ] Crear modelo LeccionCompletada con propiedades:
  - Id, UsuarioId (FK), LeccionId (FK), FechaCompletada
- [ ] Configurar relaciones entre entidades

### Archivos a crear:
- Models/Curso.cs
- Models/Leccion.cs
- Models/Inscripcion.cs
- Models/LeccionCompletada.cs

### Comando para commit:
```bash
git add .
git commit -m "Commit 4: Implementar modelos de datos - Curso, Lección, Inscripción, LecciónCompletada"
```

---

## 📋 FASE 5 (Commit 5): Configurar ASP.NET Core Identity

### Tareas a realizar:
- [ ] Instalar paquete Microsoft.AspNetCore.Identity.EntityFrameworkCore
- [ ] Modificar ApplicationDbContext para heredar de IdentityDbContext
- [ ] Configurar Identity en Program.cs
- [ ] Definir roles: Invitado, Suscriptor, Administrador
- [ ] Configurar opciones de Identity (contraseñas, lockout, etc.)

### Archivos a modificar:
- Data/ApplicationDbContext.cs
- Program.cs
- PortalMicrocursos.csproj

### Comando para commit:
```bash
git add .
git commit -m "Commit 5: Configurar ASP.NET Core Identity con roles"
```

---

## 📋 FASE 6 (Commit 6): Crear migración inicial de base de datos

### Tareas a realizar:
- [ ] Agregar DbSet para todas las entidades en ApplicationDbContext
- [ ] Crear migración inicial: `dotnet ef migrations add MigracionInicial`
- [ ] Aplicar migración: `dotnet ef database update`
- [ ] Crear clase SeedData para datos iniciales
- [ ] Agregar usuario administrador por defecto
- [ ] Agregar roles por defecto
- [ ] (Opcional) Agregar cursos de ejemplo

### Archivos a crear/modificar:
- Migrations/XXXXXX_MigracionInicial.cs (generado automáticamente)
- Data/SeedData.cs
- Program.cs (llamar a SeedData)

### Comando para commit:
```bash
git add .
git commit -m "Commit 6: Crear migración inicial y seed data"
```

---

## 📋 FASE 7 (Commit 7): Implementar repositorios (Parte 1)

### Tareas a realizar:
- [ ] Crear interfaz IRepository<T> genérica
- [ ] Crear clase Repository<T> genérica
- [ ] Crear interfaz ICursoRepository
- [ ] Crear clase CursoRepository con métodos:
  - ObtenerTodosPublicados()
  - ObtenerPorId(int id)
  - Crear(Curso curso)
  - Actualizar(Curso curso)
  - Eliminar(int id)
- [ ] Crear interfaz ILeccionRepository
- [ ] Crear clase LeccionRepository con métodos:
  - ObtenerPorCurso(int cursoId)
  - ObtenerPorId(int id)
  - Crear(Leccion leccion)
  - Actualizar(Leccion leccion)
  - Eliminar(int id)

### Archivos a crear:
- Repositories/IRepository.cs
- Repositories/Repository.cs
- Repositories/ICursoRepository.cs
- Repositories/CursoRepository.cs
- Repositories/ILeccionRepository.cs
- Repositories/LeccionRepository.cs

### Comando para commit:
```bash
git add .
git commit -m "Commit 7: Implementar repositorios de Curso y Lección"
```

---

## 📋 FASE 8 (Commit 8): Implementar repositorios (Parte 2)

### Tareas a realizar:
- [ ] Crear interfaz IInscripcionRepository
- [ ] Crear clase InscripcionRepository con métodos:
  - ObtenerPorUsuario(string usuarioId)
  - ObtenerPorCurso(int cursoId)
  - VerificarInscripcion(string usuarioId, int cursoId)
  - Crear(Inscripcion inscripcion)
- [ ] Crear interfaz ILeccionCompletadaRepository
- [ ] Crear clase LeccionCompletadaRepository con métodos:
  - ObtenerPorUsuarioYCurso(string usuarioId, int cursoId)
  - MarcarComoCompletada(string usuarioId, int leccionId)
  - VerificarCompletada(string usuarioId, int leccionId)
- [ ] Crear interfaz IUnitOfWork
- [ ] Crear clase UnitOfWork que agrupa todos los repositorios

### Archivos a crear:
- Repositories/IInscripcionRepository.cs
- Repositories/InscripcionRepository.cs
- Repositories/ILeccionCompletadaRepository.cs
- Repositories/LeccionCompletadaRepository.cs
- Repositories/IUnitOfWork.cs
- Repositories/UnitOfWork.cs

### Comando para commit:
```bash
git add .
git commit -m "Commit 8: Implementar repositorios de Inscripción, LecciónCompletada y UnitOfWork"
```

---

## 📋 FASE 9 (Commit 9): Implementar servicios de negocio (Parte 1)

### Tareas a realizar:
- [ ] Crear interfaz ICursoService
- [ ] Crear clase CursoService con métodos:
  - ObtenerCursosPublicados()
  - ObtenerCursoPorId(int id)
  - CrearCurso(Curso curso)
  - ActualizarCurso(Curso curso)
  - EliminarCurso(int id)
  - PublicarCurso(int id)
  - DespublicarCurso(int id)
- [ ] Crear interfaz ILeccionService
- [ ] Crear clase LeccionService con métodos:
  - ObtenerLeccionesPorCurso(int cursoId)
  - ObtenerLeccionPorId(int id)
  - CrearLeccion(Leccion leccion)
  - ActualizarLeccion(Leccion leccion)
  - EliminarLeccion(int id)
  - ReordenarLecciones(int cursoId, List<int> ordenIds)
- [ ] Registrar servicios en Program.cs

### Archivos a crear:
- Services/ICursoService.cs
- Services/CursoService.cs
- Services/ILeccionService.cs
- Services/LeccionService.cs

### Archivos a modificar:
- Program.cs

### Comando para commit:
```bash
git add .
git commit -m "Commit 9: Implementar servicios de Curso y Lección"
```

---

## 📋 FASE 10 (Commit 10): Implementar servicios de negocio (Parte 2)

### Tareas a realizar:
- [ ] Crear interfaz IInscripcionService
- [ ] Crear clase InscripcionService con métodos:
  - InscribirUsuario(string usuarioId, int cursoId)
  - ObtenerCursosInscritos(string usuarioId)
  - ObtenerSuscriptoresPorCurso(int cursoId)
  - VerificarAcceso(string usuarioId, int cursoId)
- [ ] Crear interfaz IProgresoService
- [ ] Crear clase ProgresoService con métodos:
  - MarcarLeccionCompletada(string usuarioId, int leccionId)
  - ObtenerProgresoUsuario(string usuarioId, int cursoId)
  - VerificarLeccionCompletada(string usuarioId, int leccionId)
  - PuedeAccederLeccion(string usuarioId, int leccionId)
- [ ] Registrar servicios en Program.cs
- [ ] Verificar que todo compile correctamente

### Archivos a crear:
- Services/IInscripcionService.cs
- Services/InscripcionService.cs
- Services/IProgresoService.cs
- Services/ProgresoService.cs

### Archivos a modificar:
- Program.cs

### Comando para commit:
```bash
git add .
git commit -m "Commit 10: Implementar servicios de Inscripción y Progreso - Backend completo"
```

---

## 🎯 Después del Commit 10

Una vez completadas estas 10 fases, habrás terminado toda la infraestructura backend del proyecto:
- ✅ Configuración base
- ✅ Entity Framework y SQL Server
- ✅ Modelos de datos completos
- ✅ ASP.NET Core Identity configurado
- ✅ Base de datos creada con migraciones
- ✅ Repositorios implementados
- ✅ Servicios de negocio implementados

Tu compañero Abigael podrá comenzar con los commits 11-20 que implementan:
- Controllers
- Views
- Funcionalidades de frontend
- Panel de administrador

---

## 📝 Notas Importantes

1. **Cada fase debe completarse antes de pasar a la siguiente**
2. **Hacer commit al finalizar cada fase**
3. **Probar que el proyecto compile después de cada commit**
4. **Consultar con tu compañero si hay dudas sobre la integración**

---

## 🚀 Comandos Útiles

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run

# Crear migración
dotnet ef migrations add NombreMigracion

# Aplicar migración
dotnet ef database update

# Ver migraciones
dotnet ef migrations list
```
