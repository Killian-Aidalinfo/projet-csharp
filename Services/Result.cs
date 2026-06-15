namespace ProjetCsharp.Services;

/// <summary>
/// Résultat d'une opération de service : soit un succès avec une valeur,
/// soit un échec avec un message d'erreur. Évite d'utiliser les exceptions
/// pour le contrôle de flux et rend les services faciles à tester.
/// </summary>
public record Result<T>(bool Success, T? Value, string? Error)
{
    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}
