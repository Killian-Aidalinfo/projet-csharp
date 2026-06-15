using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ProjetCsharp.Models;

namespace ProjetCsharp.Services;

/// <summary>
/// Gère les catégories dans un stockage en mémoire thread-safe.
/// </summary>
public partial class CategoryService
{
    private readonly ConcurrentDictionary<int, Category> _categories = new();
    private int _nextId;

    /// <summary>Toutes les catégories, triées par Id.</summary>
    public IEnumerable<Category> GetAll() => _categories.Values.OrderBy(c => c.Id);

    /// <summary>Une catégorie par son Id, ou null si elle n'existe pas.</summary>
    public Category? GetById(int id) => _categories.TryGetValue(id, out var cat) ? cat : null;

    /// <summary>
    /// Crée une catégorie après validation. Le nom est obligatoire et
    /// la couleur doit être un code hexadécimal valide (ex : "#FF0000").
    /// </summary>
    public Result<Category> Create(CreateCategoryRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return Result<Category>.Fail("Name is required");

        if (!IsValidHexColor(req.Color))
            return Result<Category>.Fail("Color must be a valid hex code (e.g. #FF0000)");

        var id = Interlocked.Increment(ref _nextId);
        var category = new Category(id, req.Name.Trim(), req.Color);
        _categories[id] = category;
        return Result<Category>.Ok(category);
    }

    /// <summary>Vrai si la chaîne est un code couleur hex à 3 ou 6 chiffres.</summary>
    public static bool IsValidHexColor(string? color) =>
        !string.IsNullOrWhiteSpace(color) && HexColorRegex().IsMatch(color);

    [GeneratedRegex("^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
    private static partial Regex HexColorRegex();
}
