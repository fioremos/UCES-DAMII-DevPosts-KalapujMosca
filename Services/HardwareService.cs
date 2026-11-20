using DevPostsApp.Models;
using Microsoft.Extensions.Logging;
namespace DevPostsApp.Services;

/// <summary>
/// Implementación concreta del servicio de hardware utilizando Microsoft.Maui.Essentials.
/// Gestiona permisos en tiempo de ejecución, almacenamiento seguro de imágenes y manejo de excepciones.
/// </summary>
public class HardwareService : IHardwareService
{
    private readonly ILogger<HardwareService> _logger;

    public HardwareService(ILogger<HardwareService> logger)
    {
        _logger = logger;
    }

    public async Task<ResultadoOperacion<string>> CapturarOSeleccionarFotoAsync()
    {
        try
        {
            FileResult? foto = null;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                // 1. Verificar y solicitar permiso de Cámara en tiempo de ejecución
                var estadoCamara = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (estadoCamara != PermissionStatus.Granted)
                {
                    estadoCamara = await Permissions.RequestAsync<Permissions.Camera>();
                }

                if (estadoCamara != PermissionStatus.Granted)
                {
                    return ResultadoOperacion<string>.Fallo(
                        "Permiso de cámara denegado. Podés habilitarlo desde los ajustes del dispositivo.",
                        TipoError.ErrorCliente);
                }

                foto = await MediaPicker.Default.CapturePhotoAsync();
            }
            else
            {
                // Fallback a galería en dispositivos sin cámara física (ej. emuladores o PC)
                foto = await MediaPicker.Default.PickPhotoAsync();
            }

            if (foto is null)
            {
                return ResultadoOperacion<string>.Fallo(
                    "Operación cancelada por el usuario.",
                    TipoError.ErrorCliente);
            }

            // 2. Persistir la imagen en el directorio privado de la app cerrando streams
            var nombreArchivo = $"post_{Guid.NewGuid():N}{Path.GetExtension(foto.FileName)}";
            var rutaDestino = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);

            using var streamOrigen = await foto.OpenReadAsync();
            using var streamDestino = File.OpenWrite(rutaDestino);
            await streamOrigen.CopyToAsync(streamDestino);

            return ResultadoOperacion<string>.Ok(rutaDestino, "Imagen adjuntada correctamente.");
        }
        catch (FeatureNotSupportedException)
        {
            return ResultadoOperacion<string>.Fallo(
                "La cámara o galería no está soportada en este dispositivo.",
                TipoError.ErrorCliente);
        }
        catch (PermissionException)
        {
            return ResultadoOperacion<string>.Fallo(
                "No se otorgaron los permisos necesarios para acceder a multimedia.",
                TipoError.ErrorCliente);
        }
        catch (Exception ex)
        {
            return ResultadoOperacion<string>.Fallo(
                $"Error inesperado al procesar la imagen: {ex.Message}",
                TipoError.FormatoInvalido);
        }
    }

    public async Task<ResultadoOperacion<string>> ObtenerUbicacionActualAsync()
    {
        try
        {
            // 1. Verificar y solicitar permiso de ubicación en tiempo de ejecución
            var estadoUbicacion = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (estadoUbicacion != PermissionStatus.Granted)
            {
                estadoUbicacion = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (estadoUbicacion != PermissionStatus.Granted)
            {
                return ResultadoOperacion<string>.Fallo(
                    "Permiso de ubicación denegado por el usuario.",
                    TipoError.ErrorCliente);
            }

            // 2. Consultar ubicación con precisión media (equilibrio batería/velocidad) y timeout de 10s
            var solicitud = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var ubicacion = await Geolocation.Default.GetLocationAsync(solicitud);

            // Fallback a última ubicación conocida si el GPS tarda en fijar satélite
            ubicacion ??= await Geolocation.Default.GetLastKnownLocationAsync();

            if (ubicacion is null)
            {
                return ResultadoOperacion<string>.Fallo(
                    "No se pudo determinar la ubicación actual. Verificá que el GPS esté encendido.",
                    TipoError.SinConexion);
            }

            var coordenadasFormateadas = $"Lat: {ubicacion.Latitude:F4}, Lon: {ubicacion.Longitude:F4}";
            return ResultadoOperacion<string>.Ok(coordenadasFormateadas, "Ubicación GPS obtenida.");
        }
        catch (FeatureNotSupportedException)
        {
            return ResultadoOperacion<string>.Fallo(
                "El sensor GPS no está disponible en este dispositivo.",
                TipoError.ErrorCliente);
        }
        catch (FeatureNotEnabledException)
        {
            return ResultadoOperacion<string>.Fallo(
                "Los servicios de ubicación (GPS) están desactivados en el sistema.",
                TipoError.ErrorCliente);
        }
        catch (PermissionException)
        {
            return ResultadoOperacion<string>.Fallo(
                "Permiso de geolocalización denegado.",
                TipoError.ErrorCliente);
        }
        catch (Exception ex)
        {
            return ResultadoOperacion<string>.Fallo(
                $"Error al obtener coordenadas GPS: {ex.Message}",
                TipoError.FormatoInvalido);
        }
    }

    public void VibrarConfirmacion(int milisegundos = 250)
    {
        try
        {
            if (Vibration.Default.IsSupported)
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(milisegundos));
                _logger.LogInformation("Vibración ejecutada correctamente ({Milisegundos} ms).", milisegundos);
            }
            else
            {
                _logger.LogInformation("Vibración omitida: el dispositivo actual no soporta retroalimentación háptica.");
            }
        }
        catch (FeatureNotSupportedException ex)
        {
            _logger.LogWarning(ex, "El motor de vibración no está soportado en esta plataforma.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al intentar activar la vibración del dispositivo.");
        }
    }
}