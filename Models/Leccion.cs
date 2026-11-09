using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Modelo que representa una lección dentro de un curso
    /// </summary>
    public class Leccion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título de la lección es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido de la lección es obligatorio")]
        public string Contenido { get; set; } = string.Empty; // Texto o HTML sencillo

        [Required(ErrorMessage = "El orden es obligatorio")]
        [Range(1, 1000, ErrorMessage = "El orden debe estar entre 1 y 1000")]
        public int Orden { get; set; } // Orden de la lección dentro del curso

        // Clave foránea
        [Required]
        public int CursoId { get; set; }

        // Propiedad de navegación: Una lección pertenece a un curso
        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }

        // Propiedad de navegación: Una lección puede estar completada por muchos usuarios
        public virtual ICollection<LeccionCompletada>? LeccionesCompletadas { get; set; }
    }
}
