using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevPostsApp.Models;
using DevPostsApp.Services;

namespace DevPostsApp.ViewModels;

/// <summary>
/// ViewModel responsable de la lógica de presentación y orquestación de la pantalla principal (<see cref="Views.MainPage"/>).
/// Hereda de <see cref="ObservableObject"/> del CommunityToolkit.Mvvm para gestión reactiva de estado mediante Source Generators,
/// y consume el servicio de red desacoplado <see cref="IPostService"/> a través de Inyección de Dependencias.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IPostService _postService;

    /// <summary>
    /// Campo de respaldo para el indicador de actividad y bloqueo de interacción.
    /// Mediante Source Generators, genera la propiedad pública reactiva <c>EstaOcupado</c>.
    /// Notifica automáticamente a la propiedad calculada <see cref="NoEstaOcupado"/> y
    /// reevalúa la condición de ejecución de <see cref="CargarPostsCommand"/>.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoEstaOcupado))]
    [NotifyCanExecuteChangedFor(nameof(CargarPostsCommand))]
    private bool _estaOcupado;

    /// <summary>
    /// Propiedad booleana inversa a <see cref="EstaOcupado"/> empleada como guarda declarativa (CanExecute)
    /// para deshabilitar el botón de carga mientras una operación asíncrona de red esté en curso.
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

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MainViewModel"/> resolviendo sus dependencias mediante el contenedor de DI.
    /// </summary>
    /// <param name="postService">Contrato de servicio para operaciones de red contra la API de publicaciones.</param>
    public MainViewModel(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Orquesta la petición asíncrona a la API REST de publicaciones, actualizando los estados visuales,
    /// poblando la colección <see cref="Posts"/> y mapeando la respuesta del servicio con códigos de color contextuales.
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
                ColorEstado = Colors.ForestGreen; // Verde para éxito
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