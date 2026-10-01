using System.Text.Json.Serialization;

namespace DevPostsApp.Models;

/// <summary>
/// Representa la entidad de una publicación obtenida desde la API REST.
/// </summary>
public class Post
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}