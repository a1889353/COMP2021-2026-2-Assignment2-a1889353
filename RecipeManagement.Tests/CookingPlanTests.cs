using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// several unit tests for the cooking plan held in the LinkedList
/// covers basic functions such as appending in order, removal and traversal
/// plus the edge cases of a missing recipe and one that is already planned
/// </summary>
public sealed class CookingPlanTests
{
    [Fact]
    public void AddRecipeToCookingPlan_ExistingRecipes_AppendsInTheOrderAdded()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Pizza"),
            SampleRecipes.Create(3, "Pasta"));

        Assert.True(manager.AddRecipeToCookingPlan(2));
        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.True(manager.AddRecipeToCookingPlan(3));

        Assert.Equal(new[] { 2, 1, 3 }, manager.GetCookingPlan());
        Assert.Equal(3, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_MissingRecipe_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        Assert.False(manager.AddRecipeToCookingPlan(100));
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_AlreadyPlanned_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.AddRecipeToCookingPlan(1));
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_MiddleOfPlan_KeepsRemainingOrder()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Pizza"),
            SampleRecipes.Create(3, "Pasta"));
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.AddRecipeToCookingPlan(3);

        Assert.True(manager.RemoveRecipeFromCookingPlan(2));

        Assert.Equal(new[] { 1, 3 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_NotPlanned_ReturnsFalseAndLeavesStackUnchanged()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RemoveRecipeFromCookingPlan(100));
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void GetCookingPlan_ReturnsCopyUnaffectedByLaterAdditions()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.Create(1, "Pancakes"),
            SampleRecipes.Create(2, "Pizza"));
        manager.AddRecipeToCookingPlan(1);

        IReadOnlyList<int> snapshot = manager.GetCookingPlan();
        manager.AddRecipeToCookingPlan(2);

        Assert.Single(snapshot);
        Assert.Equal(2, manager.CookingPlanCount);
    }
}