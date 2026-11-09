using Microsoft.AspNetCore.Identity;

namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Modelo de usuario personalizado que extiende IdentityUser
    /// ASP.NET Core Identity provee las propiedades base:
    /// - Id, UserName, Email, PasswordHash, PhoneNumber, etc.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Propiedades adicionales personalizadas para nuestro portal
        public string? NombreCompleto { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Navegación: Relación con inscripciones y lecciones completadas
        public virtual ICollection<Inscripcion>? Inscripciones { get; set; }
        public virtual ICollection<LeccionCompletada>? LeccionesCompletadas { get; set; }
    }
}
