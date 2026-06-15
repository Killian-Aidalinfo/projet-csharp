namespace ProjetCsharp.Models;

/// <summary>
/// Un projet regroupe plusieurs tâches.
/// </summary>
public record Project(
    int Id,
    string Name,
    string Description = "");
