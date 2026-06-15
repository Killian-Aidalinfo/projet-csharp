using ProjetCsharp.Models;
using ProjetCsharp.Services;
using Xunit;

namespace ProjetCsharp.Tests;

public class ProjectServiceTests
{
    [Fact]
    public void Create_WithValidName_ReturnsCreatedProject()
    {
        var svc = new ProjectService();

        var result = svc.Create(new CreateProjectRequest("Refonte du site", "Projet 2026"));

        Assert.True(result.Success);
        Assert.Equal("Refonte du site", result.Value!.Name);
        Assert.Equal("Projet 2026", result.Value.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_Fails(string? name)
    {
        var svc = new ProjectService();

        var result = svc.Create(new CreateProjectRequest(name!));

        Assert.False(result.Success);
        Assert.Equal("Name is required", result.Error);
    }

    [Fact]
    public void Create_DefaultsDescriptionToEmpty()
    {
        var svc = new ProjectService();

        var result = svc.Create(new CreateProjectRequest("Sans description"));

        Assert.Equal("", result.Value!.Description);
    }

    [Fact]
    public void Create_AssignsIncrementingIds()
    {
        var svc = new ProjectService();

        var first = svc.Create(new CreateProjectRequest("A")).Value!;
        var second = svc.Create(new CreateProjectRequest("B")).Value!;

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var svc = new ProjectService();

        Assert.Null(svc.GetById(42));
    }

    [Fact]
    public void GetAll_ReturnsProjectsOrderedById()
    {
        var svc = new ProjectService();
        svc.Create(new CreateProjectRequest("Un"));
        svc.Create(new CreateProjectRequest("Deux"));

        var ids = svc.GetAll().Select(p => p.Id).ToArray();

        Assert.Equal(new[] { 1, 2 }, ids);
    }
}
