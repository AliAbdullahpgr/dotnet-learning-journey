# DSA Two Pointers Technique

The two pointers technique uses two indices that move through a sorted array or list, usually from opposite ends or at different speeds, to solve a problem in one pass instead of using nested loops.

## Key points

- It works best on sorted arrays, or on linked lists where a fast pointer and a slow pointer can detect a cycle or find a middle element.
- Start one pointer at the beginning (`left = 0`) and the other at the end (`right = n - 1`), then move them towards each other based on the comparison with the target.
- Each step discards part of the search space, so the time complexity is usually O(n) with O(1) extra space.
- Common uses: finding a pair that sums to a target, removing duplicates from a sorted array, checking whether a string is a palindrome, and finding the middle of a linked list.
- Watch out for off-by-one errors: stop when `left < right`, not `left <= right`, unless the problem needs to check the element where the pointers meet.

## Example: pair sum in a sorted array

```csharp
bool HasPairSum(int[] nums, int target)
{
    int left = 0;
    int right = nums.Length - 1;

    while (left < right)
    {
        int sum = nums[left] + nums[right];
        if (sum == target) return true;
        if (sum < target) left++;
        else right--;
    }

    return false;
}
```

For `nums = [1, 2, 4, 7, 11]` and `target = 9`, the pointers start at 1 and 11 (sum 12, too large, so `right--`), then 1 and 7 (sum 8, too small, so `left++`), then 2 and 7 (sum 9 - a match).
