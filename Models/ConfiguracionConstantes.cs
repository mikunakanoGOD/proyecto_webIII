namespace PortalMicrocursos.Models;

/// <summary>
/// Constantes de configuración del sistema
/// </summary>
public static class ConfiguracionConstantes
{
    // Límites del sistema
    public const int MaximoCursosPorUsuario = 50;
    public const int MaximoLeccionesPorCurso = 100;
    public const int DuracionMinimaCurso = 5; // minutos
    public const int DuracionMaximaCurso = 3000; // minutos (50 horas)

    // Longitudes de campos
    public const int LongitudMaximaTitulo = 200;
    public const int LongitudMaximaDescripcion = 2000;
    public const int LongitudMaximaContenido = 50000;
    public const int LongitudMaximaNombreCompleto = 100;

    // Niveles de curso
    public static readonly string[] NivelesValidos = { "Básico", "Intermedio", "Avanzado" };

    // Mensajes del sistema
    public const string MensajeCursoPublicado = "El curso ha sido publicado exitosamente";
    public const string MensajeCursoDespublicado = "El curso ha sido despublicado";
    public const string MensajeInscripcionExitosa = "Te has inscrito exitosamente al curso";
    public const string MensajeLeccionCompletada = "¡Lección completada! Continúa con tu aprendizaje";
    public const string MensajeCursoCompletado = "¡Felicitaciones! Has completado el curso";

    // Errores comunes
    public const string ErrorCursoNoEncontrado = "El curso solicitado no existe";
    public const string ErrorLeccionNoEncontrada = "La lección solicitada no existe";
    public const string ErrorAccesoDenegado = "No tienes acceso a este recurso";
    public const string ErrorInscripcionDuplicada = "Ya estás inscrito en este curso";
    public const string ErrorCursoSinLecciones = "El curso debe tener al menos una lección para publicarse";
    public const string ErrorCursoConInscripciones = "No se puede eliminar un curso con usuarios inscritos";
}
