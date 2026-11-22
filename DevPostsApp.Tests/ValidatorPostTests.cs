using DevPostsApp.Models;
using DevPostsApp.Validators;

namespace DevPostsApp.Tests;

public class ValidadorPostTests
{
    [Fact]
    public void Validar_PostNulo_DebeRetornarFalseConMensajeDescriptivo()
    {
        Post? post = null;

        bool esValido = ValidadorPost.Validar(post, out string mensajeError);

        Assert.False(esValido);
        Assert.Equal("La publicación no puede ser nula.", mensajeError);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public void Validar_UserIdInvalido_DebeRetornarFalse(int userIdInvalido)
    {
        var post = new Post
        {
            UserId = userIdInvalido,
            Title = "Título válido de prueba",
            Body = "Contenido suficientemente largo para cumplir la regla."
        };

        bool esValido = ValidadorPost.Validar(post, out string mensajeError);

        Assert.False(esValido);
        Assert.Contains("identificador de usuario", mensajeError, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Abcd")] // Menor a 5 caracteres
    public void EsTituloValido_TitulosInvalidos_DebeRetornarFalse(string? tituloInvalido)
    {
        bool resultado = ValidadorPost.EsTituloValido(tituloInvalido);

        Assert.False(resultado);
    }

    [Fact]
    public void EsTituloValido_TituloExcedeLongitudMaxima_DebeRetornarFalse()
    {
        string tituloMuyLargo = new string('A', ValidadorPost.LongitudMaximaTitulo + 1);

        bool resultado = ValidadorPost.EsTituloValido(tituloMuyLargo);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Corto")] // Menor a 10 caracteres
    public void EsCuerpoValido_CuerposInsuficientes_DebeRetornarFalse(string? cuerpoInvalido)
    {
        bool resultado = ValidadorPost.EsCuerpoValido(cuerpoInvalido);

        Assert.False(resultado);
    }

    [Fact]
    public void Validar_PostValidoCompleto_DebeRetornarTrueSinError()
    {
        var postValido = new Post
        {
            Id = 1,
            UserId = 1,
            Title = "Arquitectura MVVM en .NET MAUI",
            Body = "Implementación de repositorio offline-first con SQLite y xUnit."
        };

        bool esValido = ValidadorPost.Validar(postValido, out string mensajeError);

        Assert.True(esValido);
        Assert.Equal(string.Empty, mensajeError);
    }
}