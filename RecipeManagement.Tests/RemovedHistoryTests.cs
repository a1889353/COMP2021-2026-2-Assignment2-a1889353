using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Unit tests for the removed recipe history held in the Stack
/// covers the expected behaviour of pushing to stack after removal operations, peeking without popping and the LIFO restore order
/// plus several edge case tests for an empty stack and a recipe that no longer exists
/// </summary>
public sealed class RemovedHistoryTests
{
    [Fact]
    public void PeekLastRemovedRecipe_EmptyStack_ReturnsNull()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_EmptyStack_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.EmptyManager();

        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_Successful_PushesTheIdOntoTheStack()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Pizza"));
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);

        Assert.True(manager.RemoveRecipeFromCookingPlan(2));

        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(2, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void PeekLastRemovedRecipe_DoesNotRemoveTheId()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_RestoresMostRecentFirstToTheEndOfThePlan()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Pizza"),
            SampleRecipes.Create(3, "Pasta"));
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(3);
        manager.RemoveRecipeFromCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(2);

        Assert.True(manager.RestoreLastRemovedRecipe());

        Assert.Equal(new[] { 3, 2 }, manager.GetCookingPlan());
        Assert.Equal(1, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_RecipeNoLongerInCatalogue_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);
        manager.RemoveRecipe(1);

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }
}