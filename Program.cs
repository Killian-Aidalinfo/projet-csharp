using ProjetCsharp.Models;
using ProjetCsharp.Services;

var builder = WebApplication.CreateBuilder(args);

// Services enregistrés en singleton : ils portent le stockage en mémoire.
builder.Services.AddSingleton<TaskService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<CategoryService>();

var app = builder.Build();

// Données de démo (l'état est réinitialisé à chaque redémarrage).
var seedTasks = app.Services.GetRequiredService<TaskService>();
seedTasks.Create(new CreateTaskRequest("Apprendre Docker Compose"));
seedTasks.Create(new CreateTaskRequest("Ecrire une API minimale en C#"));

// --- Health check ---
app.MapGet("/", () => Results.Ok(new { status = "ok", service = "projet-csharp" }));

// --- Tasks ---
app.MapGet("/tasks", (TaskService svc) => Results.Ok(svc.GetAll()));

app.MapGet("/tasks/{id:int}", (int id, TaskService svc) =>
    svc.GetById(id) is { } task
        ? Results.Ok(task)
        : Results.NotFound(new { error = $"Task {id} not found" }));

app.MapPost("/tasks", (CreateTaskRequest req, TaskService svc) =>
{
    var result = svc.Create(req);
    return result.Success
        ? Results.Created($"/tasks/{result.Value!.Id}", result.Value)
        : Results.BadRequest(new { error = result.Error });
});

// --- Projects ---
app.MapGet("/projects", (ProjectService svc) => Results.Ok(svc.GetAll()));

app.MapGet("/projects/{id:int}", (int id, ProjectService svc) =>
    svc.GetById(id) is { } project
        ? Results.Ok(project)
        : Results.NotFound(new { error = $"Project {id} not found" }));

app.MapPost("/projects", (CreateProjectRequest req, ProjectService svc) =>
{
    var result = svc.Create(req);
    return result.Success
        ? Results.Created($"/projects/{result.Value!.Id}", result.Value)
        : Results.BadRequest(new { error = result.Error });
});

// --- Categories ---
app.MapGet("/categories", (CategoryService svc) => Results.Ok(svc.GetAll()));

app.MapGet("/categories/{id:int}", (int id, CategoryService svc) =>
    svc.GetById(id) is { } category
        ? Results.Ok(category)
        : Results.NotFound(new { error = $"Category {id} not found" }));

app.MapPost("/categories", (CreateCategoryRequest req, CategoryService svc) =>
{
    var result = svc.Create(req);
    return result.Success
        ? Results.Created($"/categories/{result.Value!.Id}", result.Value)
        : Results.BadRequest(new { error = result.Error });
});

app.Run();

// Rend la classe Program accessible aux tests d'intégration éventuels.
public partial class Program { }
