using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// unit tests for the saved recipes feature via the HashSet collection
/// cases include adding, duplicate prevention, removal and membership checks
/// </summary>
public sealed class SavedRecipeTests
{
    [Fact]
    public void AddSavedRecipe_ExistingRecipe_ReturnsTrueAndSaves()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        bool added = manager.AddSavedRecipe(1);
        
        Assert.True(added);
        Assert.True(manager.IsRecipeSaved(1));
    }

    [Fact]
    public void AddSavedRecipe_MissingRecipe_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        bool added = manager.AddSavedRecipe(99);

        Assert.False(added);
        Assert.False(manager.IsRecipeSaved(99));
    }

    [Fact]
    public void AddSavedRecipe_AlreadySavedRecipe_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddSavedRecipe(1);

        bool addedAgain = manager.AddSavedRecipe(1);

        Assert.False(addedAgain);
        Assert.Equal(1, Assert.Single(manager.GetSavedRecipes()));
    }

    [Fact]
    public void RemoveSavedRecipe_SavedRecipe_ReturnsTrueAndUnsaves()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddSavedRecipe(1);

        bool removed = manager.RemoveSavedRecipe(1);

        Assert.True(removed);
        Assert.False(manager.IsRecipeSaved(1));
    }

    [Fact]
    public void RemoveSavedRecipe_NotSaved_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        bool removed = manager.RemoveSavedRecipe(1);

        Assert.False(removed);
    }

    [Fact]
    public void GetSavedRecipes_NothingSaved_ReturnsEmpty()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void GetSavedRecipes_MultipleSaved_ReturnsAll()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Waffles"),
            SampleRecipes.Create(3, "Cheesecake"));

        manager.AddSavedRecipe(3);
        manager.AddSavedRecipe(1);

        IReadOnlyList<int> saved = manager.GetSavedRecipes();

        Assert.Equal(new[] { 1, 3 }, saved.Order());
    }
}