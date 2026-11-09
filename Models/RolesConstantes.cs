namespace PortalMicrocursos.Models
{
    /// <summary>
    /// Constantes para los roles del sistema
    /// ASP.NET Core Identity maneja los roles internamente mediante IdentityRole
    /// </summary>
    public static class RolesConstantes
    {
        /// <summary>
        /// Rol Invitado: Usuario no autenticado o sin rol específico
        /// - Puede ver el catálogo de cursos
        /// - Puede ver descripciones de cursos
        /// - NO puede acceder a lecciones
        /// </summary>
        public const string Invitado = "Invitado";

        /// <summary>
        /// Rol Suscriptor: Usuario autenticado registrado en el sistema
        /// - Puede inscribirse en cursos gratuitos
        /// - Puede acceder a lecciones de sus cursos inscritos
        /// - Puede marcar lecciones como completadas
        /// - Puede consultar su progreso
        /// </summary>
        public const string Suscriptor = "Suscriptor";

        /// <summary>
        /// Rol Administrador: Usuario con permisos administrativos
        /// - Puede crear, editar y eliminar cursos
        /// - Puede crear, editar y eliminar lecciones
        /// - Puede publicar o despublicar cursos
        /// - Puede consultar suscriptores inscritos en cada curso
        /// - Puede gestionar contenido del portal
        /// </summary>
        public const string Administrador = "Administrador";

        /// <summary>
        /// Obtiene todos los roles disponibles en el sistema
        /// </summary>
        public static string[] ObtenerTodosLosRoles()
        {
            return new[] { Invitado, Suscriptor, Administrador };
        }
    }
}
