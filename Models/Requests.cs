namespace ProjetCsharp.Models;

// DTOs reçus en entrée des routes POST. Ils ne portent pas d'Id :
// l'id est généré côté serveur par les services.

public record CreateTaskRequest(
    string Title,
    bool Done = false,
    int? ProjectId = null,
    int? CategoryId = null);

public record CreateProjectRequest(
    string Name,
    string Description = "");

public record CreateCategoryRequest(
    string Name,
    string Color = "#808080");
