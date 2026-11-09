# Portal de Microcursos y Tutoriales

## Información del Proyecto

**Institución:** Universidad del Valle (Univalle)  
**Materia:** Desarrollo Web III  
**Semestre:** Cuarto Semestre  

### Estudiantes
- **Edwin Migurl Zambrana Siles**
- **Abigael Erquicia Carvajal**

---

## Descripción del Proyecto

Portal web diseñado para la publicación de microcursos y tutoriales técnicos dirigido a la comunidad de Univalle. El sistema permite a los visitantes visualizar el catálogo de cursos, mientras que los suscriptores pueden inscribirse y acceder al contenido completo de las lecciones.

### Tecnologías Utilizadas

- **Backend:** ASP.NET Core (MVC)
- **ORM:** Entity Framework Core
- **Base de Datos:** SQL Server
- **Autenticación:** ASP.NET Core Identity
- **Frontend:** Razor Pages con HTML/CSS

---

## Funcionalidades por Rol

### Invitado (Usuario no autenticado)
- Ver el catálogo de cursos publicados
- Consultar título, descripción, nivel y duración de cada curso
- Ver una página de presentación de cada curso
- **No puede** acceder a las lecciones completas

### Suscriptor (Usuario autenticado)
- Inscribirse en un curso gratuito
- Consultar sus cursos inscritos
- Acceder a las lecciones de sus cursos
- Marcar una lección como completada
- Consultar su progreso básico

### Administrador
- Crear, editar y eliminar cursos
- Agregar y ordenar las lecciones de un curso
- Publicar o despublicar cursos
- Consultar la lista de suscriptores inscritos en cada curso

---

## Modelo de Datos Principal

- **Curso:** Información del curso (título, descripción, nivel, duración, estado publicado)
- **Lección:** Contenido de cada lección dentro de un curso
- **Inscripción:** Relación entre usuario y curso
- **LecciónCompletada:** Seguimiento del progreso del usuario

Los usuarios y roles se gestionan mediante las tablas de **ASP.NET Core Identity**.

---

## Características de Seguridad

- El sistema impide que un usuario no inscrito acceda al contenido de las lecciones protegidas, incluso si conoce la URL directa
- Autenticación mediante ASP.NET Core Identity
- Autorización basada en roles (Invitado, Suscriptor, Administrador)
- Las inscripciones deben verificarse en el servidor antes de permitir el acceso

---

## Simplificaciones del Proyecto

Para reducir el alcance del desarrollo, el contenido de las lecciones será **texto o HTML sencillo**, sin incluir:
- Videos alojados
- Pagos o certificaciones
- Evaluaciones complejas

---

## Justificación Técnica

### ¿Por qué ASP.NET Core MVC?

Elegimos **ASP.NET Core MVC** por las siguientes razones:

1. **Arquitectura robusta:** El patrón MVC (Modelo-Vista-Controlador) permite separar claramente la lógica de negocio, la presentación y el control de flujo, facilitando el mantenimiento y escalabilidad del proyecto.

2. **Integración nativa con Identity:** ASP.NET Core Identity ofrece un sistema completo de autenticación y autorización que se integra perfectamente con el framework, permitiendo gestionar usuarios, roles y claims de forma segura y estándar.

3. **Entity Framework Core:** La integración con EF Core simplifica enormemente el acceso a datos, permitiendo trabajar con objetos .NET en lugar de SQL directo, con soporte para migraciones, relaciones y consultas LINQ.

4. **Seguridad incorporada:** El framework incluye protecciones contra vulnerabilidades comunes como CSRF, XSS, SQL Injection, y facilita la implementación de políticas de autorización granulares.

5. **Experiencia académica:** ASP.NET Core es ampliamente utilizado en la industria y su aprendizaje proporciona habilidades valiosas para el desarrollo profesional.

6. **Ecosistema maduro:** Cuenta con excelente documentación, herramientas de desarrollo (Visual Studio), y una comunidad activa que facilita la resolución de problemas.

### ¿Por qué no Razor Pages?

Aunque Razor Pages es más simple para aplicaciones pequeñas, MVC ofrece mayor flexibilidad para un proyecto con múltiples roles, lógica de negocio compleja y potencial crecimiento futuro.

---

## Instrucciones de Instalación

### Requisitos Previos
- .NET SDK 6.0 o superior
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code con extensiones de C#

### Pasos de Instalación

1. Clonar el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd proyecto
   ```

2. Restaurar paquetes:
   ```bash
   dotnet restore
   ```

3. Configurar la cadena de conexión en `appsettings.json`

4. Aplicar migraciones:
   ```bash
   dotnet ef database update
   ```

5. Ejecutar el proyecto:
   ```bash
   dotnet run
   ```

6. Abrir en el navegador: `https://localhost:5001`

---

## Estructura del Proyecto

```
proyecto/
├── Controllers/       # Controladores MVC
├── Models/           # Entidades y ViewModels
├── Views/            # Vistas Razor
├── Data/             # DbContext y configuraciones
├── Services/         # Lógica de negocio
├── Repositories/     # Acceso a datos
├── wwwroot/          # Archivos estáticos (CSS, JS, imágenes)
└── README.md         # Este archivo
```

---

## Progreso del Desarrollo

### Commits Realizados
- [x] **Commit 1:** Inicializar proyecto y configuración base
- [x] **Commit 2:** Configurar Entity Framework y SQL Server
- [x] **Commit 3:** Implementar modelos de datos (Parte 1)
- [x] **Commit 4:** Implementar modelos de datos (Parte 2)
- [x] **Commit 5:** Configurar ASP.NET Core Identity
- [x] **Commit 6:** Crear migración inicial de base de datos
- [x] **Commit 7:** Implementar repositorios (Parte 1)
- [x] **Commit 8:** Implementar repositorios (Parte 2)
- [x] **Commit 9:** Implementar servicios de negocio (Parte 1)
- [x] **Commit 10:** Implementar servicios de negocio (Parte 2) - **BACKEND COMPLETO** ✅

---

## Licencia

Este proyecto es un trabajo académico para la Universidad del Valle.

---

## Contacto

Para preguntas o sugerencias sobre el proyecto, contactar a los estudiantes mencionados arriba.
