using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevPostsApp.Models;
using DevPostsApp.Services;

namespace DevPostsApp.ViewModels;

/// <summary>
/// ViewModel responsable de la lógica de presentación y orquestación de la pantalla principal (<see cref="Views.MainPage"/>).
/// Hereda de <see cref="ObservableObject"/> del CommunityToolkit.Mvvm para gestión reactiva de estado mediante Source Generators,
/// y consume los servicios desacoplados <see cref="IPostService"/> e <see cref="IHardwareService"/> a través de Inyección de Dependencias.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IPostService _postService;
    private readonly IHardwareService _hardwareService;

    /// <summary>
    /// Campo de respaldo para el indicador de actividad y bloqueo de interacción.
    /// Mediante Source Generators, genera la propiedad pública reactiva <c>EstaOcupado</c>.
    /// Notifica automáticamente a la propiedad calculada <see cref="NoEstaOcupado"/> y
    /// reevalúa la condición de ejecución de los comandos asíncronos.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoEstaOcupado))]
    [NotifyCanExecuteChangedFor(nameof(CargarPostsCommand))]
    [NotifyCanExecuteChangedFor(nameof(CrearPostCommand))]
    [NotifyCanExecuteChangedFor(nameof(AdjuntarFotoCommand))]
    [NotifyCanExecuteChangedFor(nameof(CapturarUbicacionCommand))]
    private bool _estaOcupado;

    /// <summary>
    /// Propiedad booleana inversa a <see cref="EstaOcupado"/> empleada como guarda declarativa (CanExecute)
    /// para deshabilitar los botones mientras una operación asíncrona esté en curso.
    /// </summary>
    public bool NoEstaOcupado => !EstaOcupado;

    /// <summary>
    /// Mensaje textual contextual exhibido en la interfaz para informar al usuario sobre
    /// la acción requerida, el progreso en curso, el éxito de la consulta o la causa del fallo.
    /// Genera la propiedad pública <c>MensajeEstado</c>.
    /// </summary>
    [ObservableProperty]
    private string _mensajeEstado = "Presione el botón para consultar publicaciones.";

    /// <summary>
    /// Color contextual dinámico asignado al mensaje y al borde del banner en la vista.
    /// Asigna verde para éxito, rojo para desconexión o timeout, y naranja para errores HTTP.
    /// Genera la propiedad pública <c>ColorEstado</c>.
    /// </summary>
    [ObservableProperty]
    private Color _colorEstado = Colors.Gray;

    /// <summary>
    /// Colección reactiva de publicaciones mostrada en el <see cref="CollectionView"/> de la vista.
    /// Notifica dinámicamente adiciones o limpiezas de elementos sin reconstruir la colección completa.
    /// </summary>
    public ObservableCollection<Post> Posts { get; } = new();

    [ObservableProperty]
    private string _nuevoTitulo = string.Empty;

    [ObservableProperty]
    private string _nuevoCuerpo = string.Empty;

    [ObservableProperty]
    private bool _mostrarFormulario;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneImagenAdjunta))]
    private string? _rutaImagenAdjunta;

    public bool TieneImagenAdjunta => !string.IsNullOrWhiteSpace(RutaImagenAdjunta);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneUbicacion))]
    private string? _ubicacionCapturada;

    public bool TieneUbicacion => !string.IsNullOrWhiteSpace(UbicacionCapturada);

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MainViewModel"/> resolviendo sus dependencias mediante el contenedor de DI.
    /// </summary>
    /// <param name="postService">Contrato de servicio para operaciones de red y persistencia SQLite de publicaciones.</param>
    /// <param name="hardwareService">Contrato de abstracción para acceso a cámara/galería, GPS y vibración.</param>
    public MainViewModel(IPostService postService, IHardwareService hardwareService)
    {
        _postService = postService;
        _hardwareService = hardwareService;
    }

    [RelayCommand]
    private void AlternarFormulario()
    {
        MostrarFormulario = !MostrarFormulario;
    }

    [RelayCommand(CanExecute = nameof(NoEstaOcupado))]
    private async Task AdjuntarFotoAsync()
    {
        EstaOcupado = true;
        MensajeEstado = "Accediendo a cámara / galería...";
        ColorEstado = Colors.DodgerBlue;

        try
        {
            var resultado = await _hardwareService.CapturarOSeleccionarFotoAsync();
            if (resultado.Exitoso && !string.IsNullOrWhiteSpace(resultado.Datos))
            {
                RutaImagenAdjunta = resultado.Datos;
                MensajeEstado = resultado.Mensaje;
                ColorEstado = Colors.ForestGreen;
            }
            else
            {
                MensajeEstado = resultado.Mensaje;
                ColorEstado = Colors.DarkOrange;
            }
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand(CanExecute = nameof(NoEstaOcupado))]
    private async Task CapturarUbicacionAsync()
    {
        EstaOcupado = true;
        MensajeEstado = "Obteniendo coordenadas GPS...";
        ColorEstado = Colors.DodgerBlue;

        try
        {
            var resultado = await _hardwareService.ObtenerUbicacionActualAsync();
            if (resultado.Exitoso && !string.IsNullOrWhiteSpace(resultado.Datos))
            {
                UbicacionCapturada = resultado.Datos;
                MensajeEstado = $"{resultado.Mensaje} ({resultado.Datos})";
                ColorEstado = Colors.ForestGreen;
            }
            else
            {
                MensajeEstado = resultado.Mensaje;
                ColorEstado = Colors.DarkOrange;
            }
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    /// <summary>
    /// Orquesta la petición asíncrona a la API REST de publicaciones y respaldo local SQLite,
    /// actualizando los estados visuales, poblando la colección <see cref="Posts"/> y mapeando la respuesta.
    /// Genera automáticamente la propiedad pública <c>CargarPostsCommand</c> de tipo <see cref="IAsyncRelayCommand"/>.
    /// </summary>
    /// <returns>Tarea asíncrona que representa el ciclo completo de consulta y actualización de interfaz.</returns>
    [RelayCommand(CanExecute = nameof(NoEstaOcupado))]
    private async Task CargarPostsAsync()
    {
        if (EstaOcupado) return;

        EstaOcupado = true;
        MensajeEstado = "Consultando servicio remoto...";
        ColorEstado = Colors.DodgerBlue;

        try
        {
            var resultado = await _postService.ObtenerPostsAsync();

            if (resultado.Exitoso && resultado.Datos != null)
            {
                Posts.Clear();
                foreach (var post in resultado.Datos)
                {
                    Posts.Add(post);
                }

                MensajeEstado = resultado.Mensaje;
                ColorEstado = resultado.Mensaje.Contains("offline", StringComparison.OrdinalIgnoreCase)
                    ? Colors.DarkOrange
                    : Colors.ForestGreen;
            }
            else
            {
                MensajeEstado = resultado.Mensaje;

                // Mapeo semántico de error a color de interfaz
                ColorEstado = resultado.Error switch
                {
                    TipoError.SinConexion => Colors.Crimson,       // Rojo para fallos de conectividad y timeout
                    TipoError.ErrorServidor => Colors.DarkOrange,   // Naranja para errores 5xx del servidor
                    TipoError.ErrorCliente => Colors.OrangeRed,     // Naranja rojizo para errores 4xx del cliente
                    _ => Colors.DarkRed
                };
            }
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error no controlado en la aplicación: {ex.Message}";
            ColorEstado = Colors.Crimson;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand(CanExecute = nameof(NoEstaOcupado))]
    private async Task CrearPostAsync()
    {
        EstaOcupado = true;

        try
        {
            var nuevoPost = new Post
            {
                UserId = 1,
                Title = NuevoTitulo.Trim(),
                Body = NuevoCuerpo.Trim(),
                RutaImagenLocal = RutaImagenAdjunta,
                UbicacionGps = UbicacionCapturada,
                EsFavorito = true
            };

            var resultado = await _postService.CrearPostLocalAsync(nuevoPost);

            if (resultado.Exitoso && resultado.Datos is not null)
            {
                Posts.Insert(0, resultado.Datos);
                NuevoTitulo = string.Empty;
                NuevoCuerpo = string.Empty;
                RutaImagenAdjunta = null;
                UbicacionCapturada = null;
                MostrarFormulario = false;
                MensajeEstado = resultado.Mensaje;
                ColorEstado = Colors.SeaGreen;

                // Retroalimentación háptica al persistir correctamente en SQLite
                _hardwareService.VibrarConfirmacion();
            }
            else
            {
                // Muestra el error devuelto por ValidadorPost
                MensajeEstado = resultado.Mensaje;
                ColorEstado = Colors.Crimson;
            }
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    /// <summary>
    /// Gestiona la transición hacia la vista de detalle (<see cref="Views.DetallePage"/>) transfiriendo
    /// la entidad seleccionada <see cref="Post"/> completa a través de <see cref="ShellNavigationQueryParameters"/>.
    /// Genera automáticamente la propiedad pública <c>SeleccionarPostCommand</c> de tipo <see cref="IAsyncRelayCommand{Post}"/>.
    /// </summary>
    /// <param name="post">Publicación seleccionada por el usuario desde la lista.</param>
    /// <returns>Tarea asíncrona que representa la navegación por rutas de Shell.</returns>
    [RelayCommand]
    private async Task SeleccionarPostAsync(Post? post)
    {
        if (post == null) return;

        var parametros = new ShellNavigationQueryParameters
        {
            { "PostSeleccionado", post }
        };

        await Shell.Current.GoToAsync("DetallePage", parametros);
    }
}