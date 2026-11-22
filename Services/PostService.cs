using System.Net;
using System.Text.Json;
using DevPostsApp.Models;
using DevPostsApp.Repositories;
using DevPostsApp.Validators;

namespace DevPostsApp.Services;

public class PostService : IPostService
{
    private readonly HttpClient _httpClient;
    private readonly IPostRepository _postRepository;
    private const string ApiUrl = "https://jsonplaceholder.typicode.com/posts";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PostService(HttpClient httpClient, IPostRepository postRepository)
    {
        _httpClient = httpClient;
        _postRepository = postRepository;
    }

    public async Task<ResultadoOperacion<List<Post>>> ObtenerPostsAsync()
    {
        try
        {
            using var respuesta = await _httpClient.GetAsync(ApiUrl);

            if (respuesta.IsSuccessStatusCode)
            {
                var contenido = await respuesta.Content.ReadAsStringAsync();
                var postsRemotos = JsonSerializer.Deserialize<List<Post>>(contenido, _jsonOptions) ?? new();

                // Filtramos solo los posts válidos y los persistimos en lote en SQLite
                var postsValidos = postsRemotos
                    .Where(p => ValidadorPost.Validar(p, out _))
                    .ToList();

                await _postRepository.GuardarLoteAsync(postsValidos);

                // Devolvemos la lectura consolidada desde SQLite (incluye los creados localmente)
                var todosLosPosts = await _postRepository.ObtenerTodosAsync();
                return ResultadoOperacion<List<Post>>.Ok(
                    todosLosPosts.OrderByDescending(p => p.Id).ToList(),
                    $"Sincronizado correctamente ({todosLosPosts.Count} publicaciones)."
                );
            }

            // Si el servidor devolvió error HTTP (4xx / 5xx), intentamos respaldo local
            return await RecuperarDesdeCacheLocalAsync(
                $"El servidor respondió {(int)respuesta.StatusCode}. Mostrando datos locales.",
                ClasificarTipoErrorHttp(respuesta.StatusCode)
            );
        }
        catch (HttpRequestException)
        {
            return await RecuperarDesdeCacheLocalAsync(
                "Sin conexión a internet. Mostrando publicaciones guardadas offline.",
                TipoError.SinConexion
            );
        }
        catch (TaskCanceledException)
        {
            return await RecuperarDesdeCacheLocalAsync(
                "Tiempo de espera agotado. Mostrando publicaciones guardadas offline.",
                TipoError.SinConexion
            );
        }
        catch (Exception ex)
        {
            return ResultadoOperacion<List<Post>>.Fallo(
                $"Error inesperado al procesar datos: {ex.Message}",
                TipoError.FormatoInvalido
            );
        }
    }

    public async Task<ResultadoOperacion<Post>> CrearPostLocalAsync(Post nuevoPost)
    {
        // 1. Validación centralizada de dominio antes de tocar la base de datos
        if (!ValidadorPost.Validar(nuevoPost, out string mensajeError))
        {
            return ResultadoOperacion<Post>.Fallo(mensajeError, TipoError.ErrorCliente);
        }

        // 2. Generación de ID incremental si es una creación nueva (Id == 0)
        if (nuevoPost.Id <= 0)
        {
            var existentes = await _postRepository.ObtenerTodosAsync();
            nuevoPost.Id = existentes.Count > 0 ? existentes.Max(p => p.Id) + 1 : 101;
        }

        try
        {
            await _postRepository.GuardarAsync(nuevoPost);
            return ResultadoOperacion<Post>.Ok(nuevoPost, "Publicación guardada localmente.");
        }
        catch (Exception ex)
        {
            return ResultadoOperacion<Post>.Fallo(
                $"No se pudo guardar la publicación: {ex.Message}",
                TipoError.ErrorPersistencia);
        }
    }

    private async Task<ResultadoOperacion<List<Post>>> RecuperarDesdeCacheLocalAsync(
        string mensajeOffline,
        TipoError tipoErrorOriginal)
    {
        var locales = await _postRepository.ObtenerTodosAsync();

        if (locales.Count > 0)
        {
            return ResultadoOperacion<List<Post>>.Ok(
                locales.OrderByDescending(p => p.Id).ToList(),
                $"{mensajeOffline} ({locales.Count} registros)."
            );
        }

        return ResultadoOperacion<List<Post>>.Fallo(
            "No hay conexión y tampoco existen publicaciones guardadas en el dispositivo.",
            tipoErrorOriginal
        );
    }

    private static TipoError ClasificarTipoErrorHttp(HttpStatusCode statusCode)
    {
        int codigo = (int)statusCode;
        return codigo >= 500 ? TipoError.ErrorServidor : TipoError.ErrorCliente;
    }
}