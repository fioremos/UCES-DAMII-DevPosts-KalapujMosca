using System.Collections.ObjectModel;
using System.Windows.Input;
using DevPostsApp.Models;
using DevPostsApp.Services;

namespace DevPostsApp.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IPostService _postService;

    private bool _estaOcupado;
    private string _mensajeEstado = "Presione el botón para consultar publicaciones.";
    private Color _colorEstado = Colors.Gray;

    public ObservableCollection<Post> Posts { get; } = new();

    public bool EstaOcupado
    {
        get => _estaOcupado;
        set
        {
            if (SetProperty(ref _estaOcupado, value))
            {
                // Notificamos que la propiedad inversa también cambió para habilitar/deshabilitar controles
                OnPropertyChanged(nameof(NoEstaOcupado));
            }
        }
    }

    public bool NoEstaOcupado => !EstaOcupado;

    public string MensajeEstado
    {
        get => _mensajeEstado;
        set => SetProperty(ref _mensajeEstado, value);
    }

    public Color ColorEstado
    {
        get => _colorEstado;
        set => SetProperty(ref _colorEstado, value);
    }

    public ICommand CargarPostsCommand { get; }
    public ICommand SeleccionarPostCommand { get; }

    // Constructor que recibe el servicio mediante Inyección de Dependencias
    public MainViewModel(IPostService postService)
    {
        _postService = postService;

        CargarPostsCommand = new Command(async () => await CargarPostsAsync(), () => NoEstaOcupado);
        SeleccionarPostCommand = new Command<Post>(async (post) => await NavegarADetalleAsync(post));
    }

    public async Task CargarPostsAsync()
    {
        if (EstaOcupado) return;

        EstaOcupado = true;
        MensajeEstado = "Consultando servicio remoto...";
        ColorEstado = Colors.DodgerBlue;

        // Actualizamos el estado del botón
        ((Command)CargarPostsCommand).ChangeCanExecute();

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

                // Color contextual según el tipo de falla detectado
                ColorEstado = resultado.Error switch
                {
                    TipoError.SinConexion => Colors.Crimson,       // Rojo para problemas de red/timeout
                    TipoError.ErrorServidor => Colors.DarkOrange,   // Naranja para 500/servidor
                    TipoError.ErrorCliente => Colors.OrangeRed,     // Naranja rojizo para 400/404
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
            ((Command)CargarPostsCommand).ChangeCanExecute();
        }
    }

    private async Task NavegarADetalleAsync(Post? post)
    {
        if (post == null) return;

        // En la Fase 4 configuraremos la ruta Shell y el paso del objeto
        // Dejamos preparado el método para conectar en cuanto creemos DetallePage
        var parametros = new ShellNavigationQueryParameters
        {
            { "PostSeleccionado", post }
        };

        await Shell.Current.GoToAsync("DetallePage", parametros);
    }
}