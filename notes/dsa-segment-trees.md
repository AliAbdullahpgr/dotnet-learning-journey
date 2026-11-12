# Segment Trees (DSA)

A segment tree answers range queries — the sum, minimum or maximum of
`arr[l..r]` — and point updates, both in `O(log n)`. It stores the
answer for intervals in a binary tree: the root covers the whole array,
its children cover the two halves, and the leaves are single elements.

## When to use one

- **Range sum / min / max with point updates** — the classic case.
  Prefix sums answer range sums in `O(1)` but cannot handle updates;
  a segment tree handles both in `O(log n)`.
- **Range queries that are not invertible** — minimum and maximum have
  no "subtract" operation, so prefix sums do not work at all.
- If you only need sums and point updates, a Fenwick tree (BIT) is
  shorter code and less memory. Segment trees win when the operation
  is min/max, or when you later need lazy propagation for range
  updates.

## Implementation (range sum, point update)

The tree is stored in an array of size `4 * n`, which is always big
enough for any `n`:

```csharp
public sealed class SegmentTree
{
    private readonly int[] _tree;
    private readonly int _n;

    public SegmentTree(int[] arr)
    {
        _n = arr.Length;
        _tree = new int[4 * _n];
        Build(arr, 1, 0, _n - 1);
    }

    private void Build(int[] arr, int node, int left, int right)
    {
        if (left == right)
        {
            _tree[node] = arr[left];
            return;
        }
        int mid = (left + right) / 2;
        Build(arr, node * 2, left, mid);
        Build(arr, node * 2 + 1, mid + 1, right);
        _tree[node] = _tree[node * 2] + _tree[node * 2 + 1];
    }

    // Sum of arr[left..right].
    public int Query(int left, int right) => Query(1, 0, _n - 1, left, right);

    private int Query(int node, int segLeft, int segRight, int left, int right)
    {
        if (right < segLeft || left > segRight) return 0; // no overlap
        if (left <= segLeft && segRight <= right) return _tree[node]; // fully inside
        int mid = (segLeft + segRight) / 2;
        return Query(node * 2, segLeft, mid, left, right)
             + Query(node * 2 + 1, mid + 1, segRight, left, right);
    }

    // Set arr[index] = value.
    public void Update(int index, int value) => Update(1, 0, _n - 1, index, value);

    private void Update(int node, int left, int right, int index, int value)
    {
        if (left == right)
        {
            _tree[node] = value;
            return;
        }
        int mid = (left + right) / 2;
        if (index <= mid) Update(node * 2, left, mid, index, value);
        else Update(node * 2 + 1, mid + 1, right, index, value);
        _tree[node] = _tree[node * 2] + _tree[node * 2 + 1];
    }
}
```

Usage:

```csharp
var tree = new SegmentTree(new[] { 1, 3, 5, 7, 9 });
tree.Query(1, 3); // 3 + 5 + 7 = 15
tree.Update(2, 10); // arr becomes [1, 3, 10, 7, 9]
tree.Query(1, 3); // 3 + 10 + 7 = 20
```

## Complexities

| Operation | Time | Space |
| --- | --- | --- |
| Build | `O(n)` | `O(n)` (array of `4n`) |
| Range query | `O(log n)` | — |
| Point update | `O(log n)` | — |

## Takeaway

Segment trees trade a little extra memory for `O(log n)` range queries
and updates. Learn them after prefix sums: they are the answer when
the array changes between queries, or when the operation is min or
max. The next step is lazy propagation, which makes range updates
`O(log n)` too.
