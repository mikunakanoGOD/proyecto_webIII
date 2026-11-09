using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Modelo que representa el progreso de un usuario en las lecciones
    /// Registra cuando un usuario marca una lección como completada
    /// </summary>
    public class LeccionCompletada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty; // FK a AspNetUsers (Identity)

        [Required]
        public int LeccionId { get; set; }

        public DateTime FechaCompletada { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        [ForeignKey("LeccionId")]
        public virtual Leccion? Leccion { get; set; }
    }
}
