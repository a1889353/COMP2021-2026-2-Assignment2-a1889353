using System;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Unit tests for the recipe catalogue
/// Covers the expected functionality of construction, adding, finding and removing recipes
/// plus the edge cases of duplicate, missing and invalid recipes.
/// </summary>
public sealed class CatalogueTests
{
    [Fact]
    public void Constructor_NullRecipes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_DuplicateId_ThrowsArgumentException()
    {
        Recipe first = SampleRecipes.Create(1, "First Recipe");
        Recipe duplicate = SampleRecipes.Create(1, "Second Recipe");

        Assert.Throws<ArgumentException>(() => SampleRecipes.ManagerWith(first, duplicate));
    }

    [Fact]
    public void Constructor_BlankTitle_ThrowsArgumentException()
    {
        Recipe invalid = new() { Id = 2, Title = "   " };

        Assert.Throws<ArgumentException>(() => SampleRecipes.ManagerWith(invalid));
    }

    [Fact]
    public void AddRecipe_ValidRecipe_ReturnsTrueAndIncreasesCount()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        Assert.True(manager.AddRecipe(SampleRecipes.Create(3, "Pancakes")));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_DuplicateId_ReturnsFalseAndLeavesCatalogueUnchanged()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(3, "Pancakes"));

        Assert.False(manager.AddRecipe(SampleRecipes.Create(3, "Different Title")));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Equal("Pancakes", manager.FindRecipe(3)?.Title);
    }

    [Fact]
    public void AddRecipe_InvalidRecipe_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        Assert.False(manager.AddRecipe(new Recipe { Id = 0, Title = "Zero ID" }));
        Assert.False(manager.AddRecipe(new Recipe { Id = 1, Title = "   " }));
        Assert.Equal(0, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_Null_ThrowsArgumentNullException()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    public void FindRecipe_MissingId_ReturnsNull()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(4, "Pizza"));

        Assert.Null(manager.FindRecipe(100));
    }

    [Fact]
    public void RemoveRecipe_ExistingId_ReturnsTrueAndRemovesIt()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(4, "Pizza"));

        Assert.True(manager.RemoveRecipe(4));
        Assert.Equal(0, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(4));
    }

    [Fact]
    public void RemoveRecipe_MissingId_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(4, "Pizza"));

        Assert.False(manager.RemoveRecipe(100));
        Assert.Equal(1, manager.RecipeCount);
    }
}