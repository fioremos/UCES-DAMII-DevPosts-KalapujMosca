using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevPostsApp.Models;

namespace DevPostsApp.ViewModels;

/// <summary>
/// ViewModel responsable de la presentación y lógica de la vista de detalle (<see cref="Views.DetallePage"/>).
/// Hereda de <see cref="ObservableObject"/> para la reactividad de propiedades mediante Source Generators
/// e implementa <see cref="IQueryAttributable"/> para la recepción de objetos complejos transferidos vía Shell.
/// </summary>
public partial class DetalleViewModel : ObservableObject, IQueryAttributable
{
    /// <summary>
    /// Campo de respaldo para la publicación seleccionada.
    /// El atributo <see cref="ObservableProperty"/> genera automáticamente la propiedad pública
    /// <c>Post</c> notificando cambios a la interfaz mediante <c>PropertyChanged</c>.
    /// </summary>
    [ObservableProperty]
    private Post? _post;

    /// <summary>
    /// Recibe los parámetros de navegación enviados a través de <see cref="ShellNavigationQueryParameters"/>.
    /// Extrae la entidad completa <see cref="Models.Post"/> sin requerir una nueva petición de red
    /// ni fragmentar los atributos en tipos primitivos sobre la URI.
    /// </summary>
    /// <param name="query">Diccionario contextual con los parámetros de navegación provistos por Shell.</param>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("PostSeleccionado", out var postRecibido) && postRecibido is Post post)
        {
            Post = post;
        }
    }

    /// <summary>
    /// Ejecuta la navegación hacia atrás en la pila de Shell mediante la ruta relativa "..",
    /// retornando al listado principal conservando el estado y la posición previa de la colección.
    /// Genera automáticamente la propiedad pública <c>VolverCommand</c> de tipo <see cref="IAsyncRelayCommand"/>.
    /// </summary>
    /// <returns>Tarea asíncrona que representa la transición de pantalla.</returns>
    [RelayCommand]
    private async Task VolverAsync()
    {
        // Navegación hacia atrás estándar en Shell
        await Shell.Current.GoToAsync("..");
    }
}