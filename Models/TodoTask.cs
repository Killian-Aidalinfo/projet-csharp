namespace ProjetCsharp.Models;

/// <summary>
/// Une tâche à faire. ProjectId et CategoryId sont optionnels :
/// une tâche peut exister sans projet ni catégorie.
/// </summary>
public record TodoTask(
    int Id,
    string Title,
    bool Done,
    int? ProjectId = null,
    int? CategoryId = null);
