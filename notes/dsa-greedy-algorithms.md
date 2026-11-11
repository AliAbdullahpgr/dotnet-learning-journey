# Greedy Algorithms (DSA)

A greedy algorithm builds a solution step by step, always taking the
choice that looks best right now, and never reconsidering. It is fast and
simple — but it only works when the locally best choice is also part of
a globally best answer. That property is called the **greedy-choice
property**.

## Activity selection

Given activities with start and end times, pick the maximum number of
non-overlapping activities: sort by **finish time** and take each activity
that starts after the last one taken.

```csharp
// activities sorted by end time; returns the max count.
int MaxActivities((int Start, int End)[] activities)
{
    Array.Sort(activities, (a, b) => a.End.CompareTo(b.End));
    int count = 0, lastEnd = int.MinValue;
    foreach (var a in activities)
    {
        if (a.Start >= lastEnd)
        {
            count++;
            lastEnd = a.End;
        }
    }
    return count;
}
```

Picking the earliest-finishing activity leaves the most room for the
rest — that is why this greedy choice is safe.

## Classic greedy problems

- **Coin change (canonical coins)** — with 1, 5, 10, 25, always take the
  largest coin that fits. Optimal for US coins, but **not** for arbitrary
  coin systems (with coins 1, 3, 4, greedy gives 6 = 4+1+1, while 3+3 is
  better) — that version needs DP.
- **Fractional knapsack** — items can be split, so take by best
  value-per-weight first. The 0/1 version (no splitting) is **not**
  greedy-safe and needs DP.
- **Huffman coding** — repeatedly merge the two least frequent symbols;
  produces the optimal prefix code for compression.
- **Minimum number of platforms / meeting rooms** — sort arrivals and
  departures and sweep through them counting overlaps.
- **Jump game** — track the farthest reachable index; if it ever falls
  behind the current index, the end is unreachable.

## Greedy vs dynamic programming

- Greedy: one pass, no looking back, O(n log n) or O(n). Use it when the
  greedy-choice property holds.
- DP: tries the options and remembers the best, O(n^2) or more. Use it
  when a local choice can hurt the global answer (0/1 knapsack, arbitrary
  coin change, edit distance).
- When unsure, try to break the greedy idea with a small counterexample.
  If one exists, the problem is DP; if you can prove the local choice is
  always safe, greedy is the elegant answer.

## The trap

Greedy feels right even when it is wrong — it always produces *an*
answer, just not the best one. The coin-change counterexample above is
the classic reminder: always test the greedy choice against a case
designed to break it before trusting it.
