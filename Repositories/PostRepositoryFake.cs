using DevPostsApp.Models;

namespace DevPostsApp.Repositories;

/// <summary>
/// Implementación Fake en memoria del repositorio de publicaciones.
/// Utilizada para aislar dependencias en pruebas unitarias sin realizar operaciones de I/O en disco.
/// </summary>
public class PostRepositoryFake : IPostRepository
{
    private readonly List<Post> _postsEnMemoria;

    /// <summary>
    /// Inicializa el repositorio Fake con datos de prueba predefinidos opcionales.
    /// </summary>
    public PostRepositoryFake(IEnumerable<Post>? datosIniciales = null)
    {
        _postsEnMemoria = datosIniciales is not null
            ? new List<Post>(datosIniciales)
            : new List<Post>
            {
                new Post { Id = 1, UserId = 1, Title = "Post de prueba 1", Body = "Contenido inicial en memoria." },
                new Post { Id = 2, UserId = 1, Title = "Post de prueba 2", Body = "Segundo registro de prueba." }
            };
    }

    public Task<List<Post>> ObtenerTodosAsync()
    {
        // Retorna una copia de la lista para simular lectura aislada
        return Task.FromResult(_postsEnMemoria.ToList());
    }

    public Task<Post?> ObtenerPorIdAsync(int id)
    {
        var encontrado = _postsEnMemoria.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(encontrado);
    }

    public Task<int> GuardarAsync(Post post)
    {
        var indiceExistente = _postsEnMemoria.FindIndex(p => p.Id == post.Id);
        if (indiceExistente >= 0)
        {
            _postsEnMemoria[indiceExistente] = post;
        }
        else
        {
            _postsEnMemoria.Add(post);
        }
        return Task.FromResult(1);
    }

    public Task GuardarLoteAsync(IEnumerable<Post> posts)
    {
        foreach (var post in posts)
        {
            var indiceExistente = _postsEnMemoria.FindIndex(p => p.Id == post.Id);
            if (indiceExistente >= 0)
                _postsEnMemoria[indiceExistente] = post;
            else
                _postsEnMemoria.Add(post);
        }
        return Task.CompletedTask;
    }

    public Task<int> EliminarAsync(Post post)
    {
        var eliminado = _postsEnMemoria.RemoveAll(p => p.Id == post.Id);
        return Task.FromResult(eliminado);
    }
}