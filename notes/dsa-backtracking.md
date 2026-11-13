# Backtracking (DSA)

Backtracking builds a solution one choice at a time and abandons a partial solution as soon as it cannot lead to a valid answer. It is a depth-first search over the tree of all possible choices: choose, explore, then undo the choice and try the next one.

## The Template

Almost every backtracking solution has the same three steps inside a recursive function: make a choice, recurse on the smaller problem, then undo the choice so the next branch starts from a clean state.

```csharp
void Backtrack(State state)
{
    if (IsComplete(state))
    {
        Record(state);
        return;
    }

    foreach (var choice in Choices(state))
    {
        if (!IsValid(state, choice)) continue; // prune

        Apply(state, choice);     // choose
        Backtrack(state);         // explore
        Undo(state, choice);      // un-choose
    }
}
```

## Example: All Subsets

Each element is either in the subset or not. The current list is shared across calls, so it must be restored after each recursive call — and copied when recorded.

```csharp
public static List<List<int>> Subsets(int[] nums)
{
    var result = new List<List<int>>();
    var current = new List<int>();

    void Backtrack(int start)
    {
        result.Add(new List<int>(current)); // copy, not reference

        for (int i = start; i < nums.Length; i++)
        {
            current.Add(nums[i]);
            Backtrack(i + 1);
            current.RemoveAt(current.Count - 1);
        }
    }

    Backtrack(0);
    return result;
}

// Subsets([1, 2, 3]) -> [], [1], [1,2], [1,2,3], [1,3], [2], [2,3], [3]
```

## Example: Permutations

A `used` array marks which elements are already placed in the current permutation.

```csharp
public static List<List<int>> Permutations(int[] nums)
{
    var result = new List<List<int>>();
    var current = new List<int>();
    var used = new bool[nums.Length];

    void Backtrack()
    {
        if (current.Count == nums.Length)
        {
            result.Add(new List<int>(current));
            return;
        }

        for (int i = 0; i < nums.Length; i++)
        {
            if (used[i]) continue;

            used[i] = true;
            current.Add(nums[i]);
            Backtrack();
            current.RemoveAt(current.Count - 1);
            used[i] = false;
        }
    }

    Backtrack();
    return result;
}
```

## Pruning

Pruning is what separates backtracking from plain brute force. In N-Queens, a queen is placed only if no other queen shares its column or diagonal, so whole subtrees of invalid boards are never visited. In combination sum, sorting the candidates first lets the loop `break` as soon as a candidate exceeds the remaining target.

## Complexity

Backtracking is usually exponential: subsets produce 2ⁿ results and permutations produce n! results, so the work is at least the size of the output. The recursion depth is O(n), which is the extra space beyond the result. Good pruning does not change the worst case, but it often makes real inputs dramatically faster.

## Where It Is Used

Subsets, permutations and combinations, N-Queens, Sudoku solvers, word search in a grid, generating valid parentheses and partitioning a string into palindromes. Whenever a problem says "find all" or "find any valid arrangement" and the input is small, backtracking is usually the right first approach.
