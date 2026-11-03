# Binary Search (DSA)

Binary search finds a value in a **sorted** array in O(log n) time by
halving the search range on every step: compare the middle element, then
discard the half that cannot contain the target.

```csharp
int BinarySearch(int[] sorted, int target)
{
    int left = 0, right = sorted.Length - 1;
    while (left <= right)
    {
        int mid = left + (right - left) / 2; // avoids overflow vs (left + right) / 2
        if (sorted[mid] == target) return mid;
        if (sorted[mid] < target) left = mid + 1;
        else right = mid - 1;
    }
    return -1; // not found
}
```

## Worked example

Searching for 7 in `[1, 3, 5, 7, 9, 11]`:
mid = 5 (index 2) → 5 < 7, so search right half `[7, 9, 11]`;
mid = 9 (index 4) → 9 > 7, so search left half `[7]`;
mid = 7 → found at index 3. Only 3 comparisons for 6 elements.

## When it applies

- The data must be sorted first; sorting costs O(n log n), so binary
  search only pays off when you search the same collection many times.
- Variants: lower bound (first element >= target) and upper bound
  (first element > target) are the building blocks for counting
  occurrences and for insertion points in sorted lists.
- Common pitfall: an off-by-one in the loop condition. `left <= right`
  with `right = mid - 1` / `left = mid + 1` is the safe pattern above.
