using System.Collections.Concurrent;
using ProjetCsharp.Models;

namespace ProjetCsharp.Services;

/// <summary>
/// Gère les tâches dans un stockage en mémoire thread-safe.
/// L'état est perdu à chaque redémarrage (démo sans base de données).
/// </summary>
public class TaskService
{
    private readonly ConcurrentDictionary<int, TodoTask> _tasks = new();
    private int _nextId;

    /// <summary>Toutes les tâches, triées par Id.</summary>
    public IEnumerable<TodoTask> GetAll() => _tasks.Values.OrderBy(t => t.Id);

    /// <summary>Une tâche par son Id, ou null si elle n'existe pas.</summary>
    public TodoTask? GetById(int id) => _tasks.TryGetValue(id, out var task) ? task : null;

    /// <summary>
    /// Crée une tâche après validation. Le titre est obligatoire.
    /// </summary>
    public Result<TodoTask> Create(CreateTaskRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return Result<TodoTask>.Fail("Title is required");

        var id = Interlocked.Increment(ref _nextId);
        var task = new TodoTask(id, req.Title.Trim(), req.Done, req.ProjectId, req.CategoryId);
        _tasks[id] = task;
        return Result<TodoTask>.Ok(task);
    }
}
