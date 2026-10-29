using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Manages the recipe catalogue and cooking workflow using the five provided Part A collections: 
/// Dictionary, List, LinkedList, Stack and Queue + a HashSet for saved recipes in Part B
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes;
    private readonly List<string> _shoppingList = new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipes = new();
    private readonly Queue<string> _instructions = new();
    private readonly HashSet<int> _savedRecipes = new();

    /// <summary>
    /// Recipe Validation
    /// Checks for recipe being null, has a positive ID and a title that is not blank
    /// </summary>
    private static bool IsValidRecipe(Recipe? recipe) =>
        recipe is not null
        && recipe.Id > 0
        && !string.IsNullOrWhiteSpace(recipe.Title);

    /// <summary>
    /// Initialises the recipe catalogue from the provided collection of recipes
    /// Otherwise throwing an ArgumentNullException for a null collection or
    /// ArgumentException if any recipe is invalid or if contains duplicate IDs
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
    /// Returns false if the recipe is invalid or is a duplicate
    /// Throws an ArgumentNullException if it is null
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
    /// Removes a recipe from the catalogue by its associated ID
    /// Returns false if the ID is not found or if it's currently in the cooking plan, preventing the plan from holding an invalid recipe ID
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
    /// </summary>
    public IReadOnlyList<string> GetShoppingList() =>
        new List<string>(_shoppingList);

    public void ClearShoppingList() =>
        _shoppingList.Clear();

    /// <summary>
    /// Appends an existing recipe ID to the end of the cooking plan
    /// returns false if the recipe ID is not found or if it is already in the plan
    /// </summary>
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (FindRecipe(recipeId) is null || _cookingPlan.Contains(recipeId))
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    /// <summary>
    /// removes the given recipe ID from the cooking plan - pushing it onto the removed recipes stack
    /// returns false and leaves the stack unchanged if the ID is not found
    /// </summary>
    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }

        _removedRecipes.Push(recipeId);
        return true;
    }

    /// <summary>
    /// pops the recently removed recipe ID and appends it at the end of the cooking plan
    /// returns false if the stack is empty or if the recipe no longer exists
    /// </summary>
    public bool RestoreLastRemovedRecipe()
    {
        if (!_removedRecipes.TryPop(out int recipeId))
        {
            return false;
        }

        return AddRecipeToCookingPlan(recipeId);
    }

    public int? PeekLastRemovedRecipe() =>
        _removedRecipes.TryPeek(out int recipeId) ? recipeId : null;

    /// <summary>
    /// returns a copy of the cooking plan without exposing the internal linked list
    /// </summary>
    public IReadOnlyList<int> GetCookingPlan() =>
        new List<int>(_cookingPlan);

    /// <summary>
    /// Clears the current queue and loads the instructions in their original order
    /// returns false if the recipe is not found or has no instructions without altering the queue
    /// </summary>
    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe is null || recipe.Instructions.Count == 0)
        {
            return false;
        }

        _instructions.Clear();

        foreach (string instruction in recipe.Instructions)
        {
            _instructions.Enqueue(instruction);
        }

        return true;
    }

    /// <summary>
    /// returns null if the queue is empty
    /// otherwise returns the next instruction without removing it from the queue
    /// </summary>
    public string? PeekNextInstruction() =>
        _instructions.TryPeek(out string? instruction) ? instruction : null;

    /// <summary>
    /// removes and returns the first instruction from the queue
    /// returns null if the queue is empty
    /// </summary>
    public string? CompleteNextInstruction() =>
        _instructions.TryDequeue(out string? instruction) ? instruction : null;

    /// <summary>
    /// returns all recipes that contain the search term in their title and ignores case sensitivity
    /// instead returns all recipes if the search term is null, empty or whitespace
    /// </summary>
    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _recipes.Values.ToList();
        }

        string term = searchText.Trim();

        return _recipes.Values
            .Where(recipe => recipe.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// returns all recipes that contain the search term in their ingredients and ignores case sensitivity
    /// Any() treats several matching ingredients as a single match, so the recipe can only be returned once
    /// instead returns all recipes if the search term is null, empty or whitespace
    /// </summary>
    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _recipes.Values.ToList();
        }

        string term = searchText.Trim();

        return _recipes.Values
            .Where(recipe => recipe.Ingredients.Any(ingredient => ingredient.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// returns the recipes with the highest protein content, up to the specified count
    /// if recipes have identical protein values, they are ordered by ID in ascending order
    /// recipes with no nutrition information or no protein value are ignored
    /// returns an empty list if the count is zero or negative
    /// </summary>
    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return new List<Recipe>();
        }

        return _recipes.Values
            .Where(recipe => recipe.Nutrition?.ProteinG is not null)
            .OrderByDescending(recipe => recipe.Nutrition?.ProteinG)
            .ThenBy(recipe => recipe.Id)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// saves an existing recipe ID 
    /// returns false if the recipe is not in the catalogue or if it is already saved
    /// </summary>
    public bool AddSavedRecipe(int recipeId) 
    {
        if (!_recipes.ContainsKey(recipeId))
        {
            return false;
        }

        return _savedRecipes.Add(recipeId);
    }

    /// <summary>
    /// removes a recipe ID from the saved recipes
    /// returns false if the ID was not saved
    /// </summary>
    public bool RemoveSavedRecipe(int recipeId) =>
        _savedRecipes.Remove(recipeId);

    public bool IsRecipeSaved(int recipeId) =>
        _savedRecipes.Contains(recipeId);

    /// <summary>
    /// returns a copy of the saved recipes without exposing the internal HashSet
    /// </summary>
    public IReadOnlyList<int> GetSavedRecipes() =>
        new List<int>(_savedRecipes);
}
