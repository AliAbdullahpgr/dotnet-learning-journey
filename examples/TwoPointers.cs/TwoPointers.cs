// Two Pointers — find a pair that sums to a target in a sorted array.
// Run: dotnet run (as part of a console project) or paste into a C# script runner.
// Time complexity: O(n). Space complexity: O(1).

using System;

public static class TwoPointers
{
    // Returns true when two distinct elements of the sorted array add up to target.
    // The array must be sorted in ascending order.
    public static bool HasPairSum(int[] numbers, int target)
    {
        int left = 0;
        int right = numbers.Length - 1;

        while (left < right)
        {
            int sum = numbers[left] + numbers[right];

            if (sum == target)
            {
                return true;
            }

            // Sum too small: move the left pointer right to increase it.
            // Sum too large: move the right pointer left to decrease it.
            if (sum < target)
            {
                left++;
            }
            else
            {
                right--;
            }
        }

        return false;
    }

    public static void Main()
    {
        int[] numbers = { 1, 2, 4, 7, 11, 15 };

        Console.WriteLine(HasPairSum(numbers, 9));   // True  (2 + 7)
        Console.WriteLine(HasPairSum(numbers, 22));  // True  (7 + 15)
        Console.WriteLine(HasPairSum(numbers, 20));  // False (no pair sums to 20)
    }
}
