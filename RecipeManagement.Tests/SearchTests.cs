using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// tests for part b LINQ title and ingredient searches
/// cases include partial matches, whitespace and multiple matches
/// </summary>

public sealed class SearchTests
{
    [Fact]
    public void SearchByTitle_PartialMatch_ReturnsMatchingRecipes()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
           SampleRecipes.Create(1, "Pancakes"),
           SampleRecipes.Create(2, "Waffles"),
           SampleRecipes.Create(3, "Cheesecake"));
        
        IReadOnlyList<Recipe> results = manager.SearchByTitle("cake");

        Assert.Equal(new[] {1, 3}, results.Select(recipe => recipe.Id).Order());
    }

    [Fact]
    public void SearchByIngredient_Whitespace_ReturnsAllRecipes()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
           SampleRecipes.Create(1, "Waffles"),
           SampleRecipes.WithIngredients(2, "Plain Water"));

        IReadOnlyList<Recipe> results = manager.SearchByIngredient(" ");

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void SearchByIngredient_SeveralMatches_ReturnsRecipeOnce()
    {
        Recipe recipe = SampleRecipes.WithIngredients(1, "Pancakes", "2 cups milk", "1 tbsp milk powder", "2 eggs");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);

        IReadOnlyList<Recipe> results = manager.SearchByIngredient("milk");

        Assert.Same(recipe, Assert.Single(results));
    }


    [Fact]
    public void SearchByTitle_DifferentCase_ReturnsMatch()
    {
        Recipe pancakes = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(pancakes, SampleRecipes.Create(2, "Waffles"));

        IReadOnlyList<Recipe> results = manager.SearchByTitle("PANCAKES");

        Assert.Same(pancakes, Assert.Single(results));
    
    }

    [Fact]
    public void SearchByTitle_NoMatch_ReturnsEmpty()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
              SampleRecipes.Create(1, "Pancakes"),
              SampleRecipes.Create(2, "Waffles"));
        
        IReadOnlyList<Recipe> results = manager.SearchByTitle("lasagne");

        Assert.Empty(results);
    }

    [Fact]
    public void SearchByIngredient_DifferentCase_ReturnsMatch()
    {
        Recipe pancakes = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(pancakes, SampleRecipes.WithIngredients(2, "Omelette", "2 eggs", "1 tbsp butter"));

        IReadOnlyList<Recipe> results = manager.SearchByIngredient("FLOUR");

        Assert.Same(pancakes, Assert.Single(results));
    }

    [Fact]
    public void SearchByIngredient_NoMatch_ReturnsEmpty()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
              SampleRecipes.Create(1, "Pancakes"),
              SampleRecipes.WithIngredients(2, "Omelette", "2 eggs", "1 tbsp butter"));

        IReadOnlyList<Recipe> results = manager.SearchByIngredient("sugar");

        Assert.Empty(results);
    }
}


   