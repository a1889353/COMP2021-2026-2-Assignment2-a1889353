# Design

## 1. Saved recipe collection

The saved recipes are stored within a HashSet, where _savedRecipes holds the recipe IDs.

- Adding: prior to any save operation, the recipe ID is checked against the catalogue so only the recipes that actually exist can be saved.
- Duplicate prevention: adding a recipe ID that is already contained in the set just returns false and nothing is added. This way duplicates are blocked automatically.
- Removing: removing a recipe ID that isnt part of the set returns false and the set is left unchanged, so the result can be passed straight back without any extra checks.
- Membership: the recipe ID is hashed to find where it would be stored, which gives an average time complexity of O(1) for checking if a recipe is saved. The HashSet implemented doesnt keep any order, but the saved recipes dont need one, and the IDs are returned as a copy so the internal set is never exposed.

## 2. Integration

All of the Part B features are built into the existing RecipeManager and work off the same Dictionary catalogue that was designed during Part A where the constructor builds from the recipes loaded by the RecipeLoader. The title search, ingredient search and protein report use LINQ over the catalogues recipes and returns them as a new list, so it only reads from the catalogue without altering any of the Part A collections. Prior to saving a recipe, the Dictionary is used to check that the recipe ID exists, this way the saved recipes can only hold real recipes. 

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | HashSet | Average O(1) | The recipe ID is hashed to find where it would be stored, so only a few items are ever compared. |
| Add or remove a saved recipe | HashSet | Average O(1) | Uses the same hash lookup as the membership check, and adding also rejects duplicates in the same step. |
| Highest-protein report | Recipe collection | O(n log n) | Filtering out the recipes without protein is O(n), but the recipes left over then need to be sorted. |
