using ProjetCsharp.Models;
using ProjetCsharp.Services;
using Xunit;

namespace ProjetCsharp.Tests;

public class CategoryServiceTests
{
    [Fact]
    public void Create_WithValidNameAndColor_ReturnsCreatedCategory()
    {
        var svc = new CategoryService();

        var result = svc.Create(new CreateCategoryRequest("Travail", "#FF0000"));

        Assert.True(result.Success);
        Assert.Equal("Travail", result.Value!.Name);
        Assert.Equal("#FF0000", result.Value.Color);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_Fails(string? name)
    {
        var svc = new CategoryService();

        var result = svc.Create(new CreateCategoryRequest(name!));

        Assert.False(result.Success);
        Assert.Equal("Name is required", result.Error);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#GGGGGG")]
    [InlineData("#12345")]
    [InlineData("FF0000")]
    [InlineData("")]
    public void Create_WithInvalidColor_Fails(string color)
    {
        var svc = new CategoryService();

        var result = svc.Create(new CreateCategoryRequest("Perso", color));

        Assert.False(result.Success);
        Assert.Equal("Color must be a valid hex code (e.g. #FF0000)", result.Error);
    }

    [Theory]
    [InlineData("#FFF")]
    [InlineData("#abc123")]
    [InlineData("#ABCDEF")]
    public void IsValidHexColor_AcceptsValidCodes(string color)
    {
        Assert.True(CategoryService.IsValidHexColor(color));
    }

    [Theory]
    [InlineData("#FF00")]
    [InlineData("blue")]
    [InlineData(null)]
    public void IsValidHexColor_RejectsInvalidCodes(string? color)
    {
        Assert.False(CategoryService.IsValidHexColor(color));
    }

    [Fact]
    public void GetAll_ReturnsCategoriesOrderedById()
    {
        var svc = new CategoryService();
        svc.Create(new CreateCategoryRequest("Un", "#111111"));
        svc.Create(new CreateCategoryRequest("Deux", "#222222"));

        var ids = svc.GetAll().Select(c => c.Id).ToArray();

        Assert.Equal(new[] { 1, 2 }, ids);
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        var svc = new CategoryService();

        Assert.Null(svc.GetById(123));
    }
}
