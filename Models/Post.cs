using System.Text.Json.Serialization;
using SQLite;

namespace DevPostsApp.Models;

/// <summary>
/// Entidad de dominio que representa una publicación técnica.
/// Mapea tanto la respuesta JSON de la API REST como la tabla "Posts" en SQLite.
/// </summary>
[Table("Posts")]
public class Post
{
    /// <summary>
    /// Identificador único de la publicación (Clave Primaria en SQLite).
    /// </summary>
    [PrimaryKey]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del autor de la publicación.
    /// </summary>
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    /// <summary>
    /// Título de la publicación técnica.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Cuerpo o contenido completo de la publicación.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Ruta local de la imagen capturada o seleccionada desde galería (Módulo Hardware).
    /// Se ignora en la deserialización JSON externa.
    /// </summary>
    [JsonIgnore]
    public string? RutaImagenLocal { get; set; }

    /// <summary>
    /// Coordenadas GPS registradas al crear o guardar la publicación (Módulo Hardware).
    /// </summary>
    [JsonIgnore]
    public string? UbicacionGps { get; set; }

    /// <summary>
    /// Indica si el post fue marcado como favorito o creado localmente por el usuario.
    /// </summary>
    [JsonIgnore]
    public bool EsFavorito { get; set; }
}