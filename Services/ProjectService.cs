using System.Collections.Concurrent;
using ProjetCsharp.Models;

namespace ProjetCsharp.Services;

/// <summary>
/// Gère les projets dans un stockage en mémoire thread-safe.
/// </summary>
public class ProjectService
{
    private readonly ConcurrentDictionary<int, Project> _projects = new();
    private int _nextId;

    /// <summary>Tous les projets, triés par Id.</summary>
    public IEnumerable<Project> GetAll() => _projects.Values.OrderBy(p => p.Id);

    /// <summary>Un projet par son Id, ou null si il n'existe pas.</summary>
    public Project? GetById(int id) => _projects.TryGetValue(id, out var project) ? project : null;

    /// <summary>
    /// Crée un projet après validation. Le nom est obligatoire.
    /// </summary>
    public Result<Project> Create(CreateProjectRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            return Result<Project>.Fail("Name is required");

        var id = Interlocked.Increment(ref _nextId);
        var project = new Project(id, req.Name.Trim(), req.Description?.Trim() ?? "");
        _projects[id] = project;
        return Result<Project>.Ok(project);
    }
}
