// Dynamic programming in C#: Fibonacci (memoization + tabulation),
// 0/1 knapsack and coin change. Run with: dotnet run (console project)
// or paste into a C# script runner.

using System;
using System.Collections.Generic;

public static class DynamicProgramming
{
    // Fibonacci with memoization (top-down): O(n) time after caching.
    private static readonly Dictionary<int, long> _fibCache = new();

    public static long FibMemo(int n)
    {
        if (n <= 1) return n;
        if (_fibCache.TryGetValue(n, out long cached)) return cached;
        long result = FibMemo(n - 1) + FibMemo(n - 2);
        _fibCache[n] = result;
        return result;
    }

    // Fibonacci with tabulation (bottom-up): O(n) time, O(1) space.
    public static long FibTab(int n)
    {
        if (n <= 1) return n;
        long a = 0, b = 1;
        for (int i = 2; i <= n; i++)
        {
            long next = a + b;
            a = b;
            b = next;
        }
        return b;
    }

    // 0/1 knapsack: max value with the given capacity, each item at most once.
    // dp[w] = best value for capacity w. Inner loop runs backwards so an
    // item cannot be picked twice.
    public static int Knapsack(int[] weights, int[] values, int capacity)
    {
        int[] dp = new int[capacity + 1];
        for (int i = 0; i < weights.Length; i++)
            for (int w = capacity; w >= weights[i]; w--)
                dp[w] = Math.Max(dp[w], dp[w - weights[i]] + values[i]);
        return dp[capacity];
    }

    // Coin change: fewest coins to reach the amount, or -1 if impossible.
    public static int CoinChange(int[] coins, int amount)
    {
        int[] dp = new int[amount + 1];
        Array.Fill(dp, amount + 1);
        dp[0] = 0;
        for (int i = 1; i <= amount; i++)
            foreach (int coin in coins)
                if (coin <= i)
                    dp[i] = Math.Min(dp[i], dp[i - coin] + 1);
        return dp[amount] > amount ? -1 : dp[amount];
    }

    public static void Main()
    {
        Console.WriteLine($"FibMemo(40) = {FibMemo(40)}"); // 102334155
        Console.WriteLine($"FibTab(40)  = {FibTab(40)}");

        int[] weights = { 2, 3, 4, 5 };
        int[] values = { 3, 4, 5, 6 };
        Console.WriteLine($"Knapsack(cap 5) = {Knapsack(weights, values, 5)}"); // 7

        Console.WriteLine($"CoinChange([1,5,10], 28) = {CoinChange(new[] { 1, 5, 10 }, 28)}"); // 6
        Console.WriteLine($"CoinChange([2], 3) = {CoinChange(new[] { 2 }, 3)}"); // -1
    }
}
