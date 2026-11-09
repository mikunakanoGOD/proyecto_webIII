using PortalMicrocursos.Models;

namespace PortalMicrocursos.Services
{
    /// <summary>
    /// Interfaz del servicio de Progreso con lógica de negocio
    /// </summary>
    public interface IProgresoService
    {
        /// <summary>
        /// Marca una lección como completada para un usuario
        /// </summary>
        Task<LeccionCompletada> MarcarLeccionCompletadaAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Obtiene el progreso de un usuario en un curso (porcentaje)
        /// </summary>
        Task<double> ObtenerProgresoAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Obtiene información detallada del progreso de un usuario en un curso
        /// </summary>
        Task<ProgresoDetalle> ObtenerProgresoDetalladoAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Verifica si un usuario ha completado una lección
        /// </summary>
        Task<bool> EstaLeccionCompletadaAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Verifica si un usuario puede acceder a una lección (está inscrito en el curso)
        /// </summary>
        Task<bool> PuedeAccederLeccionAsync(string usuarioId, int leccionId);

        /// <summary>
        /// Obtiene los IDs de las lecciones completadas por un usuario en un curso
        /// </summary>
        Task<List<int>> ObtenerLeccionesCompletadasAsync(string usuarioId, int cursoId);

        /// <summary>
        /// Verifica si un usuario ha completado todo un curso
        /// </summary>
        Task<bool> HaCompletadoCursoAsync(string usuarioId, int cursoId);
    }

    /// <summary>
    /// Clase para encapsular el progreso detallado de un usuario en un curso
    /// </summary>
    public class ProgresoDetalle
    {
        public int CursoId { get; set; }
        public string CursoTitulo { get; set; } = string.Empty;
        public int TotalLecciones { get; set; }
        public int LeccionesCompletadas { get; set; }
        public double PorcentajeProgreso { get; set; }
        public List<int> IdsLeccionesCompletadas { get; set; } = new List<int>();
        public bool CursoCompletado { get; set; }
    }
}
