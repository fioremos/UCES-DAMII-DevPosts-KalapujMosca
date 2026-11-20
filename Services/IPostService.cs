using DevPostsApp.Models;

namespace DevPostsApp.Services;

/// <summary>
/// Contrato de abstraccion para la obtencion de publicaciones desde la API.
/// </summary>
public interface IPostService
{
    /// <summary>
    /// Obtiene publicaciones sincronizando con la API remota y persistiendo en SQLite.
    /// Ante fallos de red, recurre al almacenamiento local (Offline-First).
    /// </summary>
    Task<ResultadoOperacion<List<Post>>> ObtenerPostsAsync();

    /// <summary>
    /// Valida y persiste una nueva publicación en la base de datos local SQLite.
    /// </summary>
    Task<ResultadoOperacion<Post>> CrearPostLocalAsync(Post nuevoPost);
}