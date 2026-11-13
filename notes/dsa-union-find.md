# Union-Find (Disjoint Set Union) (DSA)

Union-Find, also called Disjoint Set Union (DSU), tracks a collection of elements partitioned into disjoint sets. It answers one question very fast: are these two elements in the same set? It supports two operations — `Union` merges two sets, and `Find` returns the representative of an element's set — both in almost-constant time.

## The Idea

Each set is a tree. Every element points to a parent, and the root of the tree is the set's representative. Two elements are in the same set when their roots are the same. Initially every element is its own root, so every element is in a set of one.

```csharp
public class UnionFind
{
    private readonly int[] _parent;
    private readonly int[] _rank;

    public UnionFind(int size)
    {
        _parent = new int[size];
        _rank = new int[size];
        for (int i = 0; i < size; i++)
            _parent[i] = i;
    }
}
```

## Find with Path Compression

`Find` follows parent pointers up to the root. Path compression flattens the tree on the way: every node visited is pointed directly at the root, so the next lookup is a single step.

```csharp
public int Find(int x)
{
    if (_parent[x] != x)
        _parent[x] = Find(_parent[x]); // path compression
    return _parent[x];
}
```

## Union by Rank

`Union` finds both roots and attaches the shorter tree under the taller one. Rank is an upper bound on the tree's height, and attaching by rank keeps the tree shallow. Together, path compression and union by rank make each operation cost O(α(n)) — the inverse Ackermann function — which is effectively constant for any input size in practice.

```csharp
public bool Union(int a, int b)
{
    int rootA = Find(a);
    int rootB = Find(b);

    if (rootA == rootB) return false; // already in the same set

    if (_rank[rootA] < _rank[rootB])
        _parent[rootA] = rootB;
    else if (_rank[rootA] > _rank[rootB])
        _parent[rootB] = rootA;
    else
    {
        _parent[rootB] = rootA;
        _rank[rootA]++;
    }
    return true;
}

public bool Connected(int a, int b) => Find(a) == Find(b);
```

## A Small Demo

```csharp
var uf = new UnionFind(5);
uf.Union(0, 1);
uf.Union(1, 2);
Console.WriteLine(uf.Connected(0, 2)); // True
Console.WriteLine(uf.Connected(0, 3)); // False
uf.Union(2, 3);
Console.WriteLine(uf.Connected(0, 3)); // True
```

## Where It Is Used

Union-Find is the standard tool for connectivity problems. It finds connected components in a graph by unioning every edge's endpoints, then counting distinct roots. Kruskal's algorithm for the minimum spanning tree sorts all edges by weight and adds each edge only if its endpoints are in different sets — exactly the `Union` return value above. It also appears in image processing (grouping connected pixels), in network connectivity checks, and in problems like "number of islands" or "friend circles". Whenever the question is "are these connected, and what happens when I connect them?", Union-Find is usually the simplest correct answer.
