using System.ComponentModel.DataAnnotations;

namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Modelo que representa un curso en el portal
    /// </summary>
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nivel es obligatorio")]
        [StringLength(50)]
        public string Nivel { get; set; } = "Básico"; // Básico, Intermedio, Avanzado

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Range(1, 10000, ErrorMessage = "La duración debe estar entre 1 y 10000 minutos")]
        public int Duracion { get; set; } // En minutos

        public bool EstaPublicado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad de navegación: Un curso tiene muchas lecciones
        public virtual ICollection<Leccion>? Lecciones { get; set; }

        // Propiedad de navegación: Un curso tiene muchas inscripciones
        public virtual ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}
