using System.Net;
using System.Text.Json;
using DevPostsApp.Models;

namespace DevPostsApp.Services;

public class PostService : IPostService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://jsonplaceholder.typicode.com/posts";

    // Recibe HttpClient por inyeccion de dependencias para evitar agotamiento de sockets
    public PostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10); // Tiempo limite de 10 segundos
    }

    public async Task<ResultadoOperacion<List<Post>>> ObtenerPostsAsync()
    {
        try
        {
            // 1. Petición GET asíncrona
            using var respuesta = await _httpClient.GetAsync(ApiUrl);

            // 2. Verificación del código de estado HTTP
            if (!respuesta.IsSuccessStatusCode)
            {
                return ClasificarErrorHttp(respuesta.StatusCode);
            }

            // 3. Lectura del contenido
            var contenidoJson = await respuesta.Content.ReadAsStringAsync();

            // 4. Deserializacion segura
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var posts = JsonSerializer.Deserialize<List<Post>>(contenidoJson, opciones);

            if (posts == null || posts.Count == 0)
            {
                return ResultadoOperacion<List<Post>>.Fallo(
                    "No se encontraron publicaciones disponibles.",
                    TipoError.ErrorServidor);
            }

            return ResultadoOperacion<List<Post>>.Ok(
                posts,
                $"Se cargaron {posts.Count} publicaciones correctamente.");
        }
        catch (TaskCanceledException)
        {
            // Ocurre cuando se agota el tiempo de espera (Timeout)
            return ResultadoOperacion<List<Post>>.Fallo(
                "La solicitud tardó demasiado tiempo en responder (Timeout). Verifique su conexión.",
                TipoError.SinConexion);
        }
        catch (HttpRequestException ex)
        {
            // Ocurre cuando el dispositivo no tiene internet, falla el DNS o la red se interrumpe
            return ResultadoOperacion<List<Post>>.Fallo(
                $"Fallo de red: No fue posible conectar con el servidor ({ex.Message}).",
                TipoError.SinConexion);
        }
        catch (JsonException)
        {
            // Ocurre si la API devuelve texto plano, HTML o JSON corrupto
            return ResultadoOperacion<List<Post>>.Fallo(
                "Error de formato: Los datos recibidos no tienen la estructura esperada.",
                TipoError.FormatoInvalido);
        }
        catch (Exception ex)
        {
            // Captura de respaldo ante cualquier imprevisto
            return ResultadoOperacion<List<Post>>.Fallo(
                $"Ocurrió un error inesperado: {ex.Message}",
                TipoError.ErrorServidor);
        }
    }

    private static ResultadoOperacion<List<Post>> ClasificarErrorHttp(HttpStatusCode codigo)
    {
        int codigoNum = (int)codigo;

        if (codigoNum >= 500)
        {
            return ResultadoOperacion<List<Post>>.Fallo(
                $"Error del servidor ({codigoNum}): El servicio externo está experimentando problemas.",
                TipoError.ErrorServidor);
        }

        if (codigo == HttpStatusCode.NotFound)
        {
            return ResultadoOperacion<List<Post>>.Fallo(
                "Recurso no encontrado (Error 404): La dirección de publicaciones no existe.",
                TipoError.ErrorCliente);
        }

        return ResultadoOperacion<List<Post>>.Fallo(
            $"Error en la solicitud ({codigoNum} {codigo}): No se pudo completar la operación.",
            TipoError.ErrorCliente);
    }
}