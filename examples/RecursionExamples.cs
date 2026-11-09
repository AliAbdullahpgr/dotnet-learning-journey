// Recursion examples: factorial and Fibonacci.
// A recursive method calls itself with a smaller input until it reaches a base case.
// Time complexity: factorial O(n); naive Fibonacci O(2^n).

using System;

public static class RecursionExamples
{
    /// <summary>
    /// Returns n! (n factorial): n * (n - 1)! with 0! = 1.
    /// </summary>
    public static long Factorial(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Factorial is not defined for negative numbers.");
        }

        if (n <= 1)
        {
            return 1; // base case
        }

        return n * Factorial(n - 1); // recursive step
    }

    /// <summary>
    /// Returns the nth Fibonacci number: Fib(0) = 0, Fib(1) = 1,
    /// Fib(n) = Fib(n - 1) + Fib(n - 2).
    /// </summary>
    public static long Fibonacci(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Fibonacci is not defined for negative numbers.");
        }

        if (n <= 1)
        {
            return n; // base case
        }

        return Fibonacci(n - 1) + Fibonacci(n - 2); // recursive step
    }

    public static void Main()
    {
        Console.WriteLine(Factorial(5));  // 120
        Console.WriteLine(Factorial(0));  // 1
        Console.WriteLine(Fibonacci(10)); // 55
        Console.WriteLine(Fibonacci(0));  // 0
    }
}
