# Time Complexity and Big O

Time complexity describes how the running time of an algorithm grows as the input size `n` grows. Big O notation gives an upper bound on that growth, so algorithms can be compared without depending on a specific machine.

## Key points

- **O(1) — constant:** accessing an array element by index; the cost does not depend on `n`.
- **O(log n) — logarithmic:** binary search; each step halves the remaining input.
- **O(n) — linear:** a single pass over the input, such as linear search.
- **O(n log n) — linearithmic:** efficient comparison sorts such as merge sort and quicksort (average case).
- **O(n^2) — quadratic:** a loop nested inside another loop over the same input.
- **O(2^n) — exponential:** naive recursive solutions that branch twice per step, such as naive Fibonacci.
- Drop constants and lower-order terms: O(2n + 5) simplifies to O(n).
- Big O describes the worst case; Big Omega describes a lower bound and Big Theta a tight bound.
- Space complexity is measured the same way, but for the extra memory an algorithm uses.

## Example

```csharp
// O(n) time, O(1) space: scans the array once
int LinearSearch(int[] values, int target)
{
    for (int i = 0; i < values.Length; i++)
    {
        if (values[i] == target) return i;
    }
    return -1;
}
```

For the same problem, binary search on a sorted array is O(log n) time and O(1) space — much faster for large inputs, but it requires the array to be sorted first.
