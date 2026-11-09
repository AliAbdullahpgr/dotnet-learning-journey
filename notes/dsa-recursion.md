# Recursion Basics

Recursion is when a function solves a problem by calling itself on a smaller version of the same problem, until it reaches a case it can answer directly.

## Key points

- Every recursive function needs a base case that stops the recursion, and a recursive case that moves towards that base case.
- Without a base case, or a recursive case that never gets smaller, the function calls itself forever and ends with a stack overflow.
- Each call gets its own stack frame with its own local variables, and the frames unwind in reverse order when the base case is reached.
- Recursion fits problems that break into smaller copies of themselves: walking a tree, searching a sorted range, or sorting by divide and conquer.
- A naive recursive Fibonacci is slow because it recomputes the same values many times; memoisation, or an iterative loop, fixes that.
- Think in three steps when writing one: what is the smallest input, how do I use the answer for a smaller input, and how does the input get smaller.
- If the depth can be very large, prefer a loop or an explicit stack, because every call uses stack space.

## Example

```csharp
int Factorial(int n)
{
    if (n == 0) return 1;          // base case
    return n * Factorial(n - 1);   // recursive case
}

Console.WriteLine(Factorial(4)); // 24
// Factorial(4) = 4 * Factorial(3) = 4 * 3 * Factorial(2)
//              = 4 * 3 * 2 * Factorial(1) = 4 * 3 * 2 * 1 * Factorial(0) = 24
```
