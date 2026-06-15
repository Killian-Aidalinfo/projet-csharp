using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory store (thread-safe) to keep the demo dependency-free.
var tasks = new ConcurrentDictionary<int, TodoTask>();
var nextId = 0;

void Seed(string title)
{
    var id = Interlocked.Increment(ref nextId);
    tasks[id] = new TodoTask(id, title, false);
}

Seed("Apprendre Docker Compose");
Seed("Ecrire une API minimale en C#");

// Route 1 — health check
app.MapGet("/", () => Results.Ok(new { status = "ok", service = "projet-csharp" }));

// Route 2 — list all tasks
app.MapGet("/tasks", () => Results.Ok(tasks.Values.OrderBy(t => t.Id)));

// Route 3 — get a single task by id
app.MapGet("/tasks/{id:int}", (int id) =>
    tasks.TryGetValue(id, out var task)
        ? Results.Ok(task)
        : Results.NotFound(new { error = $"Task {id} not found" }));

// Route 4 — create a task
app.MapPost("/tasks", (CreateTaskRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Title))
        return Results.BadRequest(new { error = "Title is required" });

    var id = Interlocked.Increment(ref nextId);
    var task = new TodoTask(id, req.Title, req.Done);
    tasks[id] = task;
    return Results.Created($"/tasks/{id}", task);
});

app.Run();

record TodoTask(int Id, string Title, bool Done);
record CreateTaskRequest(string Title, bool Done = false);
