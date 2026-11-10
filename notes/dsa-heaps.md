# Heaps (DSA)

A heap is a binary tree with one rule: every parent is smaller than its
children (min-heap) or larger than its children (max-heap). The smallest
(or largest) element is always at the root, so a heap is the right tool
whenever you repeatedly need the "most urgent" item.

## What a heap guarantees

- **Peek** at the min/max element: O(1).
- **Insert** and **remove min/max**: O(log n).
- **Search** for an arbitrary value: O(n) — a heap is not a search
  structure. Use a hash map for lookups.

## `PriorityQueue<TElement, TPriority>` in .NET

.NET ships a heap as `PriorityQueue<TElement, TPriority>` (lower priority
value comes out first):

```csharp
var queue = new PriorityQueue<string, int>();

queue.Enqueue("low priority task", 3);
queue.Enqueue("urgent task", 1);
queue.Enqueue("normal task", 2);

Console.WriteLine(queue.Dequeue()); // urgent task
Console.WriteLine(queue.Dequeue()); // normal task
Console.WriteLine(queue.Peek());    // low priority task
```

Enqueue and Dequeue are O(log n); Peek is O(1).

## How a heap works internally

A heap is stored in a plain array, not as linked nodes. For an element at
index `i`:

- parent: `(i - 1) / 2`
- left child: `2 * i + 1`
- right child: `2 * i + 2`

Inserting appends at the end and "bubbles up" while the parent breaks the
rule. Removing the root moves the last element to the root and "bubbles
down" until the rule holds again.

## Classic heap problems

- **K largest / smallest elements** — keep a heap of size K while
  scanning: O(n log k) instead of sorting everything.
- **Merging K sorted lists** — push the head of each list into a heap and
  repeatedly take the minimum: O(n log k).
- **Task scheduling** — always run the highest-priority waiting task.
- **Dijkstra's shortest path** — a min-heap always expands the closest
  unvisited vertex first.

## Heaps vs sorting

Sorting a whole array to find the top K costs O(n log n). A heap of size K
does it in O(n log k) and uses only O(k) extra memory — a real win when K
is small and n is large.
