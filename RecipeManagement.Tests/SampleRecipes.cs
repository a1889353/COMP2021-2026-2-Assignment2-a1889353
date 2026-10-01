using System;
using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;
 
/// <summary>
/// Helper class which provides sample recipes and recipe managers for use in the unit tests
/// doesn't depend on the supplied recipes.json file.
/// </summary>
internal static class SampleRecipes
{
    public static Recipe Create(int id = 1, string title = "Test Recipe") => new()
    {
        Id = id,
        Title = title,
        Ingredients = new List<string> { "400g flour", "4 eggs", "2 cup milk" },
        Instructions = new List<string> { "Mix", "Add", "Cook" }
    };

    public static Recipe WithoutInstructions(int id = 2, string title = "Pasta") => new()
    {
        Id = id,
        Title = title,
        Ingredients = new List<string> { "200g pasta", "1 cup sauce" }
    };

    public static RecipeManager EmptyManager() => new(Array.Empty<Recipe>());

    public static RecipeManager ManagerWith(params Recipe[] recipes) => new(recipes);
}