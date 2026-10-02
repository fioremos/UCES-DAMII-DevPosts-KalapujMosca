using System.Windows.Input;
using DevPostsApp.Models;

namespace DevPostsApp.ViewModels;

/// <summary>
/// ViewModel para la pantalla de detalle.
/// Implementa IQueryAttributable para recibir objetos complejos transferidos por Shell.
/// </summary>
public class DetalleViewModel : BaseViewModel, IQueryAttributable
{
    private Post? _post;

    public Post? Post
    {
        get => _post;
        set => SetProperty(ref _post, value);
    }

    public ICommand VolverCommand { get; }

    public DetalleViewModel()
    {
        VolverCommand = new Command(async () => await VolverAsync());
    }

    /// <summary>
    /// Recibe los parámetros de navegación de Shell en forma de diccionario de objetos.
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("PostSeleccionado", out var postRecibido) && postRecibido is Post post)
        {
            Post = post;
        }
    }

    private async Task VolverAsync()
    {
        // Navegación hacia atrás estándar en Shell
        await Shell.Current.GoToAsync("..");
    }
}