using DevPostsApp.Models;

namespace DevPostsApp.Services;

/// <summary>
/// Contrato de abstracción para acceder a los sensores y periféricos del dispositivo
/// (Cámara/Galería, GPS y Vibración) facilitando el desacoplamiento y las pruebas unitarias.
/// </summary>
public interface IHardwareService
{
    /// <summary>
    /// Verifica permisos en tiempo de ejecución y captura una fotografía con la cámara
    /// o selecciona una desde la galería si la cámara no está disponible (ej. escritorio).
    /// </summary>
    Task<ResultadoOperacion<string>> CapturarOSeleccionarFotoAsync();

    /// <summary>
    /// Solicita permisos de ubicación y obtiene las coordenadas GPS actuales (Latitud y Longitud).
    /// </summary>
    Task<ResultadoOperacion<string>> ObtenerUbicacionActualAsync();

    /// <summary>
    /// Emite una retroalimentación háptica breve si el dispositivo soporta vibración.
    /// </summary>
    void VibrarConfirmacion(int milisegundos = 250);
}