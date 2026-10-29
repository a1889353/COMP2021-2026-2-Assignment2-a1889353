using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// full integration test of the RecipeManager workflow
/// covers all five Part A collections as one recipe moves through each of the implemented components
/// added part b workflow tests for searches, protein reporting and saving/removing of recipes
/// </summary>
public sealed class WorkflowTests
{
    [Fact]
    public void FullWorkflow_RecipeMovesThroughEveryComponent()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.EmptyManager();

        // catalogue
        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(1, manager.RecipeCount);

        // shopping list
        Assert.Equal(recipe.Ingredients.Count, manager.AddIngredientsToShoppingList(1));
        Assert.Equal(recipe.Ingredients, manager.GetShoppingList());

        // cooking plan
        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());

        // catalogue removal refused while in cookingplan
        Assert.False(manager.RemoveRecipe(1));
        Assert.Equal(1, manager.RecipeCount);

        // removed recipe history
        Assert.True(manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());

        // restore to the end of the cooking plan
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);

        // instruction queue
        Assert.True(manager.StartCooking(1));
        Assert.Equal(recipe.Instructions.Count, manager.PendingInstructionCount);

        foreach (string expected in recipe.Instructions)
        {
            Assert.Equal(expected, manager.CompleteNextInstruction());
        }

        Assert.Equal(0, manager.PendingInstructionCount);

        // shopping list clear
        manager.ClearShoppingList();
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void PartB_WorkflowTests_SearchReportAndSave()
    {
        Recipe stroganoff = SampleRecipes.WithProtein(1, "Beef Stroganoff", 40);
        Recipe salad = SampleRecipes.WithProtein(2, "Chicken Pesto Salad", 30);
        Recipe yorkshire = SampleRecipes.Create(3, "Yorkshire Pudding");
        RecipeManager manager = SampleRecipes.ManagerWith(stroganoff, salad, yorkshire);

        // title search finds recipe from the catalogue
        Recipe found = Assert.Single(manager.SearchByTitle("stroganoff"));
        Assert.Same(stroganoff, found);

        // search result used by part a
        Assert.True(manager.AddRecipeToCookingPlan(found.Id));
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());

        // ingredient search through the same catalogue
        Assert.Same(yorkshire, Assert.Single(manager.SearchByIngredient("flour")));

        // protein report - excludes recipes with no nutrition data
        Assert.Equal(new[] { 1, 2 }, manager.GetHighestProteinRecipes(5).Select(recipe => recipe.Id));

        // saved recipes add, check and remove functions
        Assert.True(manager.AddSavedRecipe(found.Id));
        Assert.True(manager.IsRecipeSaved(1));
        Assert.True(manager.RemoveSavedRecipe(1));
        Assert.Empty(manager.GetSavedRecipes());

        // checks that part b's calls didnt alter the catalogue or cooking plan
        Assert.Equal(3, manager.RecipeCount);
        Assert.Equal(1, manager.CookingPlanCount);
    }
}