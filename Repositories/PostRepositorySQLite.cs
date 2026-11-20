using DevPostsApp.Models;
using SQLite;

namespace DevPostsApp.Repositories;

/// <summary>
/// Implementación concreta del repositorio de publicaciones sobre base de datos local SQLite.
/// Garantiza persistencia offline en el almacenamiento interno del dispositivo.
/// </summary>
public class PostRepositorySQLite : IPostRepository
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    /// <summary>
    /// Constructor por defecto para producción: utiliza el directorio privado de datos de la app.
    /// </summary>
    public PostRepositorySQLite()
        : this(Path.Combine(FileSystem.AppDataDirectory, "devposts.db3"))
    {
    }

    /// <summary>
    /// Constructor sobrecargado que permite inyectar una ruta personalizada (ej. ":memory:" para tests de integración).
    /// </summary>
    /// <param name="dbPath">Ruta física del archivo .db3 o identificador en memoria.</param>
    public PostRepositorySQLite(string dbPath)
    {
        _dbPath = dbPath;
    }

    /// <summary>
    /// Inicializa de forma perezosa (Lazy) la conexión y crea la tabla Posts si no existe.
    /// </summary>
    private async Task InitAsync()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<Post>();
    }

    public async Task<List<Post>> ObtenerTodosAsync()
    {
        await InitAsync();
        return await _database!.Table<Post>().ToListAsync();
    }

    public async Task<Post?> ObtenerPorIdAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<Post>().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> GuardarAsync(Post post)
    {
        await InitAsync();
        // InsertOrReplaceAsync inserta si el Id no existe o actualiza si ya estaba guardado
        return await _database!.InsertOrReplaceAsync(post);
    }

    public async Task GuardarLoteAsync(IEnumerable<Post> posts)
    {
        await InitAsync();
        await _database!.RunInTransactionAsync(conn =>
        {
            foreach (var post in posts)
            {
                conn.InsertOrReplace(post);
            }
        });
    }

    public async Task<int> EliminarAsync(Post post)
    {
        await InitAsync();
        return await _database!.DeleteAsync(post);
    }
}