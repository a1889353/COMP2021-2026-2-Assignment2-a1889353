using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// full integration test of the RecipeManager workflow
/// covers all five Part A collections as one recipe moves through each of the implemented components
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
}