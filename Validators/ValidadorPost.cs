using DevPostsApp.Models;

namespace DevPostsApp.Validators;

/// <summary>
/// Centraliza las reglas de validación de dominio para la entidad Post.
/// Permite reutilizar criterios de integridad y testearlos de forma aislada (Unit Testing).
/// </summary>
public static class ValidadorPost
{
	public const int LongitudMinimaTitulo = 5;
	public const int LongitudMaximaTitulo = 120;
	public const int LongitudMinimaCuerpo = 10;

	/// <summary>
	/// Valida que el título cumpla con obligatoriedad y límites de longitud.
	/// </summary>
	public static bool EsTituloValido(string? titulo)
	{
		if (string.IsNullOrWhiteSpace(titulo))
			return false;

		var limpio = titulo.Trim();
		return limpio.Length >= LongitudMinimaTitulo && limpio.Length <= LongitudMaximaTitulo;
	}

	/// <summary>
	/// Valida que el cuerpo de la publicación contenga contenido sustancial.
	/// </summary>
	public static bool EsCuerpoValido(string? cuerpo)
	{
		if (string.IsNullOrWhiteSpace(cuerpo))
			return false;

		return cuerpo.Trim().Length >= LongitudMinimaCuerpo;
	}

	/// <summary>
	/// Valida de forma integral un objeto Post antes de persistirlo o enviarlo.
	/// </summary>
	/// <param name="post">Entidad a evaluar.</param>
	/// <param name="mensajeError">Descripción del primer incumplimiento detectado.</param>
	/// <returns>True si el objeto cumple todas las reglas de negocio; False en caso contrario.</returns>
	public static bool Validar(Post? post, out string mensajeError)
	{
		if (post is null)
		{
			mensajeError = "La publicación no puede ser nula.";
			return false;
		}

		if (post.UserId <= 0)
		{
			mensajeError = "El identificador de usuario debe ser mayor a cero.";
			return false;
		}

		if (!EsTituloValido(post.Title))
		{
			mensajeError = $"El título es obligatorio y debe tener entre {LongitudMinimaTitulo} y {LongitudMaximaTitulo} caracteres.";
			return false;
		}

		if (!EsCuerpoValido(post.Body))
		{
			mensajeError = $"El contenido es obligatorio y debe tener al menos {LongitudMinimaCuerpo} caracteres.";
			return false;
		}

		mensajeError = string.Empty;
		return true;
	}
}