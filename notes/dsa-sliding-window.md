# DSA: Sliding Window

The sliding window technique solves problems on arrays and strings by keeping a "window" — a contiguous range between two pointers — and moving it across the data instead of re-checking every subarray from scratch.

## Key points

- **Two pointers:** a left pointer marks the start of the window and a right pointer expands it, one element at a time.
- **Fixed window:** the window size stays the same (for example, size `k`). When the right pointer moves forward, the left pointer moves too, so the window slides by one.
- **Variable window:** the window grows and shrinks. Expand the right pointer to include elements, then move the left pointer forward while the window breaks some condition (for example, it contains a duplicate).
- **Reuse work:** instead of recomputing a sum or a count for each subarray, update the previous window's result by adding the new element and removing the element that left the window.
- **Time complexity:** usually **O(n)**, because each element enters and leaves the window at most once. Extra space is typically **O(1)**, apart from any small bookkeeping structure (such as a frequency map).

## Common uses

- Maximum sum of a subarray of size `k`.
- Longest substring without repeating characters.
- Longest subarray with at most `k` distinct elements.
- Smallest subarray whose sum is at least a target value.

## Example in C#: maximum sum of a subarray of size k

```csharp
int MaxSumSubarrayOfSizeK(int[] nums, int k)
{
    int windowSum = 0;
    for (int i = 0; i < k; i++)
    {
        windowSum += nums[i];
    }

    int maxSum = windowSum;
    for (int i = k; i < nums.Length; i++)
    {
        windowSum += nums[i] - nums[i - k];
        maxSum = Math.Max(maxSum, windowSum);
    }

    return maxSum;
}
```

For `nums = [2, 1, 5, 1, 3, 2]` and `k = 3`, the windows are `[2, 1, 5]`, `[1, 5, 1]`, `[5, 1, 3]`, and `[1, 3, 2]`, so the answer is `9`.

## Takeaway

Reach for a sliding window when a problem asks about a contiguous subarray or substring and the answer for a slightly shifted window can be updated cheaply from the previous one.
