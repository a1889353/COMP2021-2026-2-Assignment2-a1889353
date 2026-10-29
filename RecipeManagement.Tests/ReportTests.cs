using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// unit tests for part b linq highest protein report implementation
/// cases include ordering, missing nutrition info, count limits and identical protein values
/// </summary>
public sealed class ReportTests
{
    [Fact]
    public void GetHighestProteinRecipes_SeveralRecipes_ReturnsHighestFirst()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Salad", 5),
            SampleRecipes.WithProtein(2, "Steak", 35),
            SampleRecipes.WithProtein(3, "Chicken", 25));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(3);

        Assert.Equal(new[] { 2, 3, 1 }, results.Select(recipe => recipe.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes_NullNutrition_ExcludesRecipe()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Steak", 35),
            SampleRecipes.Create(2, "Pancakes"));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(5);

        Assert.Equal(1, Assert.Single(results).Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_NullProtein_ExcludesRecipe()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Steak", 35),
            SampleRecipes.WithProtein(2, "Soup", null));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(5);

        Assert.Equal(1, Assert.Single(results).Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_ZeroCount_ReturnsEmpty()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Steak", 35));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(0);

        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_NegativeCount_ReturnsEmpty()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Salad", 5),
            SampleRecipes.WithProtein(2, "Steak", 35),
            SampleRecipes.Create(3, "Pancakes"));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(-1);

        Assert.Empty(results);
    }

    [Fact]
    public void GetHighestProteinRecipes_CountAboveAvailable_ReturnsAllRecipesWithProtein()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(1, "Salad", 5),
            SampleRecipes.WithProtein(2, "Steak", 35),
            SampleRecipes.Create(3, "Pancakes"));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(10);

        Assert.Equal(new[] { 2, 1 }, results.Select(recipe => recipe.Id));
    }

    [Fact]
    public void GetHighestProteinRecipes_IdenticalProtein_OrdersById()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(
            SampleRecipes.WithProtein(2, "Beef Stroganoff", 35),
            SampleRecipes.WithProtein(1, "Chicken Salad", 35));

        IReadOnlyList<Recipe> results = manager.GetHighestProteinRecipes(2);

        Assert.Equal(new[] { 1, 2 }, results.Select(recipe => recipe.Id));
    }
}
