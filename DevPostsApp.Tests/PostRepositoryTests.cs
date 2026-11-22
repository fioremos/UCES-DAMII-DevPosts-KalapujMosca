using DevPostsApp.Models;
using DevPostsApp.Repositories;

namespace DevPostsApp.Tests;

public class PostRepositoryTests
{
    private const string RutaDbEnMemoria = ":memory:";

    [Fact]
    public async Task GuardarAsync_PostConHardwareMetadata_DebePersistirYRecuperarCorrectamente()
    {
        var repo = new PostRepositorySQLite(RutaDbEnMemoria);
        var post = new Post
        {
            Id = 500,
            UserId = 1,
            Title = "Registro de campo con GPS",
            Body = "Publicación capturada desde dispositivo móvil con coordenadas.",
            RutaImagenLocal = "/data/user/0/devposts/foto_test.jpg",
            UbicacionGps = "Lat: -34.5300, Lon: -58.5200",
            EsFavorito = true
        };

        await repo.GuardarAsync(post);
        var recuperado = await repo.ObtenerPorIdAsync(500);

        Assert.NotNull(recuperado);
        Assert.Equal("Registro de campo con GPS", recuperado!.Title);
        Assert.Equal("/data/user/0/devposts/foto_test.jpg", recuperado.RutaImagenLocal);
        Assert.Equal("Lat: -34.5300, Lon: -58.5200", recuperado.UbicacionGps);
        Assert.True(recuperado.EsFavorito);
    }

    [Fact]
    public async Task GuardarLoteAsync_VariosPosts_DebeInsertarTodosSinDuplicados()
    {
        var repo = new PostRepositorySQLite(RutaDbEnMemoria);
        var lote = new List<Post>
        {
            new() { Id = 1, UserId = 1, Title = "Primer post", Body = "Cuerpo del primer post." },
            new() { Id = 2, UserId = 1, Title = "Segundo post", Body = "Cuerpo del segundo post." }
        };

        await repo.GuardarLoteAsync(lote);
        lote[0].Title = "Primer post actualizado";
        await repo.GuardarLoteAsync(lote);

        var todos = await repo.ObtenerTodosAsync();

        Assert.Equal(3, todos.Count);
        Assert.Contains(todos, p => p.Id == 1 && p.Title == "Primer post actualizado");
    }
}