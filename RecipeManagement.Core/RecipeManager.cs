using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _instructions = new();

    /// <summary>
    /// Recipe Validation
    /// checks that the recipe is not null, has a positive ID, and has a non-empty title
    /// </summary>
    private static bool IsValidRecipe(Recipe? recipe) =>
        recipe is not null
        && recipe.Id > 0
        && !string.IsNullOrWhiteSpace(recipe.Title);

    /// <summary>
    /// Initialises the recipe catalogue from the provided collection of recipes 
    /// Throws an ArgumentException if any recipe is deemed invalid or if there are duplicate recipe IDs
    /// </summary>
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(recipes);
        _recipes = new Dictionary<int, Recipe>();
        foreach (Recipe recipe in recipes)
        {
            if (!IsValidRecipe(recipe))
            {
                throw new ArgumentException(
                    "Each recipe must have a positive ID and a valid title.",
                    nameof(recipes));
            }

            if (!_recipes.TryAdd(recipe.Id, recipe))
            {
                throw new ArgumentException(
                    $"Recipe contains duplicate ID: {recipe.Id}, adjust accordingly.",
                    nameof(recipes));
            }
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructions.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    /// <summary>
    /// Adds a validated recipe to the catalogue
    /// Returns false if the recipe is invalid or is a duplicate which throws NullException
    /// </summary>
    public bool AddRecipe(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (!IsValidRecipe(recipe))
        {
            return false;
        }

        return _recipes.TryAdd(recipe.Id, recipe);
    }

    public Recipe? FindRecipe(int recipeId) =>
        _recipes.GetValueOrDefault(recipeId);

    /// <summary>
    /// Removes recipe from catalogue by its associated ID
    /// Returns false if the ID is not found or if its currently in the cooking plan, preventing the plan from holding an invadid recipe ID
    /// </summary>
    public bool RemoveRecipe(int recipeId)
    {
        if (_cookingPlan.Contains(recipeId))
        {
            return false;
        }

        return _recipes.Remove(recipeId);
    }

    /// <summary>
    /// Copies the ingredients from the specified recipe into the shopping list
    /// Returns the number of ingredients added or 0 if the recipe ID is not found
    /// </summary>
    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null)
        {
            return 0;    
        }

        _shoppingList.AddRange(recipe.Ingredients);
        return recipe.Ingredients.Count;
    }

    /// <summary>
    /// Returns a copy of the shopping list without exposing the internal list 
    /// this way any items added or removed from the returned list will not affect the internal shopping list
    /// </summary>
    public IReadOnlyList<string> GetShoppingList() =>
        new List<string>(_shoppingList);

    public void ClearShoppingList() =>
        _shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement AddRecipeToCookingPlan.");

    public bool RemoveRecipeFromCookingPlan(int recipeId) =>
        throw new NotImplementedException("Part A: implement RemoveRecipeFromCookingPlan.");

    public bool RestoreLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement RestoreLastRemovedRecipe.");

    public int? PeekLastRemovedRecipe() =>
        throw new NotImplementedException("Part A: implement PeekLastRemovedRecipe.");

    public IReadOnlyList<int> GetCookingPlan() =>
        throw new NotImplementedException("Part A: implement GetCookingPlan.");

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
