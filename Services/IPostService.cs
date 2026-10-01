using DevPostsApp.Models;

namespace DevPostsApp.Services;

/// <summary>
/// Contrato de abstraccion para la obtencion de publicaciones desde la API.
/// </summary>
public interface IPostService
{
    Task<ResultadoOperacion<List<Post>>> ObtenerPostsAsync();
}