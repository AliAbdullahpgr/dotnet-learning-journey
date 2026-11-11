# Dynamic Programming Basics (DSA)

Dynamic programming (DP) solves a problem by breaking it into overlapping
subproblems, solving each subproblem once, and reusing the answers. Two
conditions must hold: **optimal substructure** (the best answer is built
from the best answers of smaller parts) and **overlapping subproblems**
(the same smaller parts appear again and again).

## The two approaches

- **Memoization (top-down)** — write the natural recursion, then cache
  each result in a dictionary or array. The first call computes, later
  calls reuse. Easy to write, but recursion depth and call overhead remain.
- **Tabulation (bottom-up)** — fill a table from the smallest case up to
  the answer, no recursion at all. Usually faster and uses less stack; the
  table can often be shrunk to a few variables.

## Fibonacci: naive vs DP

```csharp
// Naive recursion: O(2^n) — Fib(5) recomputes Fib(3) twice, Fib(2) three times.
int FibNaive(int n) => n <= 1 ? n : FibNaive(n - 1) + FibNaive(n - 2);

// Tabulation: O(n) time, O(1) space — keep only the last two values.
int Fib(int n)
{
    if (n <= 1) return n;
    int a = 0, b = 1;
    for (int i = 2; i <= n; i++)
    {
        int next = a + b;
        a = b;
        b = next;
    }
    return b;
}
```

## 0/1 knapsack

Given items with a weight and a value, and a bag with a weight limit, pick
items to maximize value without exceeding the limit. Each item is taken at
most once:

```csharp
// dp[w] = best value achievable with capacity w.
int Knapsack(int[] weights, int[] values, int capacity)
{
    int[] dp = new int[capacity + 1];
    for (int i = 0; i < weights.Length; i++)
        for (int w = capacity; w >= weights[i]; w--)
            dp[w] = Math.Max(dp[w], dp[w - weights[i]] + values[i]);
    return dp[capacity];
}
```

The inner loop runs **backwards** on purpose: going forwards would let the
same item be picked more than once.

## Classic DP problems

- **Coin change** — fewest coins to reach an amount, or how many ways.
- **Longest common subsequence / edit distance** — compare two strings by
  building a table over their prefixes.
- **Climbing stairs / house robber** — one-dimensional DP where the answer
  for step `i` depends on the previous one or two steps.
- **Longest increasing subsequence** — O(n^2) with a table, O(n log n)
  with binary search.

## How to spot a DP problem

- The question asks for a **maximum, minimum, or count** over many choices.
- A brute-force recursion repeats the same calls (draw the recursion tree
  and look for duplicates).
- The answer for input `n` can be expressed from answers for smaller
  inputs — that recurrence is the heart of the solution. If the choices
  are independent and never repeat, it is usually greedy instead, not DP.
