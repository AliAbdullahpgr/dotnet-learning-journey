// Binary search on a sorted integer array.
// Requires the array to be sorted in ascending order.
// Time complexity: O(log n); space complexity: O(1).

using System;

public static class BinarySearchExample
{
    /// <summary>
    /// Returns the index of <paramref name="target"/> in a sorted array,
    /// or -1 when the target is not present.
    /// </summary>
    public static int BinarySearch(int[] sorted, int target)
    {
        int left = 0;
        int right = sorted.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (sorted[mid] == target)
            {
                return mid;
            }
            else if (sorted[mid] < target)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return -1;
    }

    public static void Main()
    {
        int[] numbers = { 2, 5, 8, 12, 16, 23, 38, 56, 72, 91 };

        Console.WriteLine(BinarySearch(numbers, 23)); // 5
        Console.WriteLine(BinarySearch(numbers, 91)); // 9
        Console.WriteLine(BinarySearch(numbers, 7));  // -1 (not found)
    }
}
