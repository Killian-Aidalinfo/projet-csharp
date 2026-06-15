using ProjetCsharp.Models;
using ProjetCsharp.Services;
using Xunit;

namespace ProjetCsharp.Tests;

public class TaskServiceTests
{
    [Fact]
    public void Create_WithValidTitle_ReturnsCreatedTask()
    {
        var svc = new TaskService();

        var result = svc.Create(new CreateTaskRequest("Faire les courses"));

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Faire les courses", result.Value!.Title);
        Assert.False(result.Value.Done);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_Fails(string? title)
    {
        var svc = new TaskService();

        var result = svc.Create(new CreateTaskRequest(title!));

        Assert.False(result.Success);
        Assert.Equal("Title is required", result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Create_TrimsTitle()
    {
        var svc = new TaskService();

        var result = svc.Create(new CreateTaskRequest("  Ranger  "));

        Assert.Equal("Ranger", result.Value!.Title);
    }

    [Fact]
    public void Create_AssignsIncrementingIds()
    {
        var svc = new TaskService();

        var first = svc.Create(new CreateTaskRequest("A")).Value!;
        var second = svc.Create(new CreateTaskRequest("B")).Value!;

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void Create_KeepsProjectAndCategoryLinks()
    {
        var svc = new TaskService();

        var result = svc.Create(new CreateTaskRequest("Liée", ProjectId: 7, CategoryId: 3));

        Assert.Equal(7, result.Value!.ProjectId);
        Assert.Equal(3, result.Value.CategoryId);
    }

    [Fact]
    public void GetById_ExistingTask_ReturnsIt()
    {
        var svc = new TaskService();
        var created = svc.Create(new CreateTaskRequest("Trouvable")).Value!;

        var found = svc.GetById(created.Id);

        Assert.Equal(created, found);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var svc = new TaskService();

        Assert.Null(svc.GetById(999));
    }

    [Fact]
    public void GetAll_ReturnsTasksOrderedById()
    {
        var svc = new TaskService();
        svc.Create(new CreateTaskRequest("Un"));
        svc.Create(new CreateTaskRequest("Deux"));
        svc.Create(new CreateTaskRequest("Trois"));

        var ids = svc.GetAll().Select(t => t.Id).ToArray();

        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public void GetAll_OnEmptyService_ReturnsEmpty()
    {
        var svc = new TaskService();

        Assert.Empty(svc.GetAll());
    }
}
