namespace DevPostsApp.Models;

/// <summary>
/// Tipos de resultados posibles para errores de red o servidor.
/// </summary>
public enum TipoError
{
    Ninguno,
    SinConexion,      // Problema de conectividad o timeout
    ErrorServidor,    // Errores HTTP 500, 503, etc.
    ErrorCliente,     // Errores HTTP 400, 404, etc.
    FormatoInvalido   // Error al deserializar JSON corrupto
}

/// <summary>
/// Contenedor para transportar los datos y el estado de la petición.
/// </summary>
public class ResultadoOperacion<T>
{
    public bool Exitoso { get; set; }
    public T? Datos { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public TipoError Error { get; set; } = TipoError.Ninguno;

    // Método helper para crear una respuesta exitosa
    public static ResultadoOperacion<T> Ok(T datos, string mensaje = "Operación exitosa")
    {
        return new ResultadoOperacion<T>
        {
            Exitoso = true,
            Datos = datos,
            Mensaje = mensaje,
            Error = TipoError.Ninguno
        };
    }

    // Método helper para crear una respuesta con error clasificado
    public static ResultadoOperacion<T> Fallo(string mensaje, TipoError tipoError)
    {
        return new ResultadoOperacion<T>
        {
            Exitoso = false,
            Datos = default,
            Mensaje = mensaje,
            Error = tipoError
        };
    }
}