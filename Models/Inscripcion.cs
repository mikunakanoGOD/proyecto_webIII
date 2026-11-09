using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Modelo que representa la inscripción de un usuario en un curso
    /// </summary>
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty; // FK a AspNetUsers (Identity)

        [Required]
        public int CursoId { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }
    }
}
