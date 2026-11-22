using System.Net;
using System.Text;
using System.Text.Json;
using DevPostsApp.Models;
using DevPostsApp.Repositories;
using DevPostsApp.Services;

namespace DevPostsApp.Tests;

public class PostServiceTests
{
    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    [Fact]
    public async Task ObtenerPostsAsync_ApiExitosa_DebeSincronizarYPersistirEnRepositorio()
    {
        var postsRemotos = new List<Post>
        {
            new() { Id = 10, UserId = 1, Title = "Post desde API", Body = "Contenido remoto válido." }
        };
        var json = JsonSerializer.Serialize(postsRemotos);

        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }));

        var fakeRepo = new PostRepositoryFake();
        int cantidadInicial = (await fakeRepo.ObtenerTodosAsync()).Count;
        var service = new PostService(httpClient, fakeRepo);

        var resultado = await service.ObtenerPostsAsync();
        var persistidos = await fakeRepo.ObtenerTodosAsync();

        Assert.True(resultado.Exitoso);
        Assert.Equal(cantidadInicial + 1, resultado.Datos!.Count);
        Assert.Contains(persistidos, p => p.Id == 10 && p.Title == "Post desde API");
    }

    [Fact]
    public async Task ObtenerPostsAsync_SinConexionConCacheLocal_DebeRecuperarPostsOffline()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
            throw new HttpRequestException("No hay conexión a internet")));

        var fakeRepo = new PostRepositoryFake();
        await fakeRepo.GuardarAsync(new Post
        {
            Id = 77,
            UserId = 1,
            Title = "Post cacheado localmente",
            Body = "Disponible sin conexión a internet."
        });

        var service = new PostService(httpClient, fakeRepo);

        var resultado = await service.ObtenerPostsAsync();

        Assert.True(resultado.Exitoso);
        Assert.Contains(resultado.Datos!, p => p.Id == 77 && p.Title == "Post cacheado localmente");
        Assert.Contains("offline", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrearPostLocalAsync_PostInvalido_NoDebePersistirYDebeRetornarError()
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
        var fakeRepo = new PostRepositoryFake();
        int cantidadInicial = (await fakeRepo.ObtenerTodosAsync()).Count;
        var service = new PostService(httpClient, fakeRepo);

        var postInvalido = new Post { UserId = 1, Title = "No", Body = "Corto" };

        var resultado = await service.CrearPostLocalAsync(postInvalido);
        var todos = await fakeRepo.ObtenerTodosAsync();

        Assert.False(resultado.Exitoso);
        Assert.Equal(TipoError.ErrorCliente, resultado.Error);
        Assert.Equal(cantidadInicial, todos.Count);
    }
}