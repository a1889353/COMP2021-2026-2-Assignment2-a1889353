using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// unit tests for the shopping list
/// covers copying ingredients in order, appending across recipes and clearing,
/// plus the edge case of a recipe ID that is not found
/// </summary>
public sealed class ShoppingListTests
{
    [Fact]
    public void AddIngredientsToShoppingList_ExistingRecipe_CopiesIngredientsInOrder()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);

        int added = manager.AddIngredientsToShoppingList(1);

        Assert.Equal(recipe.Ingredients.Count, added);
        Assert.Equal(recipe.Ingredients, manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_MissingRecipe_ReturnsZeroAndLeavesListUnchanged()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);
        manager.AddIngredientsToShoppingList(1);

        Assert.Equal(0, manager.AddIngredientsToShoppingList(100));
        Assert.Equal(recipe.Ingredients.Count, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_TwoRecipes_AppendsToTheEndOfTheList()
    {
        Recipe first = SampleRecipes.Create(1, "Pancakes");
        Recipe second = SampleRecipes.Create(2, "Pizza");
        RecipeManager manager = SampleRecipes.ManagerWith(first, second);

        manager.AddIngredientsToShoppingList(1);
        manager.AddIngredientsToShoppingList(2);

        Assert.Equal(first.Ingredients.Count + second.Ingredients.Count, manager.ShoppingItemCount);
    }

    [Fact]
    public void GetShoppingList_ReturnsCopyUnaffectedByLaterAdditions()
    {
        Recipe first = SampleRecipes.Create(1, "Pancakes");
        Recipe second = SampleRecipes.Create(2, "Pizza");
        RecipeManager manager = SampleRecipes.ManagerWith(first, second);
        manager.AddIngredientsToShoppingList(1);

        IReadOnlyList<string> snapshot = manager.GetShoppingList();
        manager.AddIngredientsToShoppingList(2);

        Assert.Equal(first.Ingredients.Count, snapshot.Count);
        Assert.Equal(first.Ingredients.Count + second.Ingredients.Count, manager.ShoppingItemCount);
    }

    [Fact]
    public void ClearShoppingList_RemovesAllItems()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));
        manager.AddIngredientsToShoppingList(1);

        manager.ClearShoppingList();

        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }
}