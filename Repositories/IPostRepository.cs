using DevPostsApp.Models;

namespace DevPostsApp.Repositories;

/// <summary>
/// Contrato de abstracción para el acceso a datos locales de publicaciones (Patrón Repositorio).
/// Desacopla la lógica de aplicación y presentación del motor de persistencia subyacente (SQLite o Fake).
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Obtiene todas las publicaciones almacenadas localmente.
    /// </summary>
    Task<List<Post>> ObtenerTodosAsync();

    /// <summary>
    /// Obtiene una publicación específica por su identificador único.
    /// </summary>
    Task<Post?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Inserta o actualiza una publicación en el almacenamiento local.
    /// </summary>
    Task<int> GuardarAsync(Post post);

    /// <summary>
    /// Inserta o actualiza un lote completo de publicaciones (útil para sincronización con la API).
    /// </summary>
    Task GuardarLoteAsync(IEnumerable<Post> posts);

    /// <summary>
    /// Elimina una publicación del almacenamiento local.
    /// </summary>
    Task<int> EliminarAsync(Post post);
}