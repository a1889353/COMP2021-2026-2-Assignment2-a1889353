using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Unit tests for the cooking instructions held in the Queue
/// covers the functionality of loading a recipe in order, peeking without dequeuing and completing one instruction at a time
/// plus the edge cases of an empty queue and a recipe that contains no instructions
/// </summary>
public sealed class InstructionQueueTests
{
    [Fact]
    public void StartCooking_ExistingRecipe_LoadsInstructionsInOrder()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);

        Assert.True(manager.StartCooking(1));
        Assert.Equal(recipe.Instructions.Count, manager.PendingInstructionCount);

        foreach (string expected in recipe.Instructions)
        {
            Assert.Equal(expected, manager.CompleteNextInstruction());
        }

        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_MissingRecipe_ReturnsFalse()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        Assert.False(manager.StartCooking(100));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_RecipeWithNoInstructions_ReturnsFalseAndKeepsCurrentQueue()
    {
        Recipe cookable = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(
            cookable,
            SampleRecipes.WithoutInstructions(2, "Pasta"));
        manager.StartCooking(1);
        manager.CompleteNextInstruction();

        Assert.False(manager.StartCooking(2));
        Assert.Equal(cookable.Instructions.Count - 1, manager.PendingInstructionCount);
        Assert.Equal(cookable.Instructions[1], manager.PeekNextInstruction());
    }

    [Fact]
    public void StartCooking_SecondRecipe_ClearsThePreviousQueue()
    {
        Recipe first = SampleRecipes.Create(1, "Pancakes");
        Recipe second = SampleRecipes.Create(2, "Pizza");
        RecipeManager manager = SampleRecipes.ManagerWith(first, second);
        manager.StartCooking(1);
        manager.CompleteNextInstruction();

        Assert.True(manager.StartCooking(2));
        Assert.Equal(second.Instructions.Count, manager.PendingInstructionCount);
        Assert.Equal(second.Instructions[0], manager.PeekNextInstruction());
    }

    [Fact]
    public void PeekNextInstruction_DoesNotRemoveTheInstruction()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);
        manager.StartCooking(1);

        Assert.Equal(recipe.Instructions[0], manager.PeekNextInstruction());
        Assert.Equal(recipe.Instructions[0], manager.PeekNextInstruction());
        Assert.Equal(recipe.Instructions.Count, manager.PendingInstructionCount);
    }

    [Fact]
    public void CompleteNextInstruction_RemovesExactlyOneFromTheFront()
    {
        Recipe recipe = SampleRecipes.Create(1, "Pancakes");
        RecipeManager manager = SampleRecipes.ManagerWith(recipe);
        manager.StartCooking(1);

        Assert.Equal(recipe.Instructions[0], manager.CompleteNextInstruction());
        Assert.Equal(recipe.Instructions.Count - 1, manager.PendingInstructionCount);
        Assert.Equal(recipe.Instructions[1], manager.PeekNextInstruction());
    }

    [Fact]
    public void PeekNextInstruction_EmptyQueue_ReturnsNull()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        Assert.Null(manager.PeekNextInstruction());
    }

    [Fact]
    public void CompleteNextInstruction_EmptyQueue_ReturnsNull()
    {
        RecipeManager manager = SampleRecipes.ManagerWith(SampleRecipes.Create(1, "Pancakes"));

        Assert.Null(manager.CompleteNextInstruction());
    }
}