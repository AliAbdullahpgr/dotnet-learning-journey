// Sliding Window — maximum sum of any contiguous subarray of size k.
// Run: dotnet run (as part of a console project) or paste into a C# script runner.
// Time complexity: O(n). Space complexity: O(1).

using System;

public static class SlidingWindow
{
    // Returns the largest sum of any k consecutive elements.
    // Throws when k is not a valid window size for the array.
    public static int MaxSumSubarrayOfSizeK(int[] numbers, int k)
    {
        if (k <= 0 || k > numbers.Length)
        {
            throw new ArgumentException("k must be between 1 and the array length.", nameof(k));
        }

        // Sum of the first window.
        int windowSum = 0;
        for (int i = 0; i < k; i++)
        {
            windowSum += numbers[i];
        }

        int maxSum = windowSum;

        // Slide the window one step at a time: drop the element that
        // leaves the window and add the element that enters it.
        for (int i = k; i < numbers.Length; i++)
        {
            windowSum += numbers[i] - numbers[i - k];
            maxSum = Math.Max(maxSum, windowSum);
        }

        return maxSum;
    }

    public static void Main()
    {
        int[] numbers = { 2, 1, 5, 1, 3, 2 };

        Console.WriteLine(MaxSumSubarrayOfSizeK(numbers, 3)); // 9  (5 + 1 + 3)
        Console.WriteLine(MaxSumSubarrayOfSizeK(numbers, 2)); // 6  (5 + 1)
    }
}
