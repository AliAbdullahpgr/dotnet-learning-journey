# Topological Sort (DSA)

Topological sort orders the vertices of a directed acyclic graph (DAG) so that for every directed edge u → v, u comes before v. It only works on DAGs — a cycle makes a valid ordering impossible.

## Where it is used

- Build order: compile a file only after its dependencies are compiled.
- Course prerequisites: take CS101 before CS201.
- Task scheduling: run a migration before the service that depends on it.
- Package managers: install dependencies before the packages that need them.

## Kahn's algorithm (BFS-based)

Count the in-degree of every vertex. Repeatedly remove a vertex with in-degree 0, append it to the result, and decrease the in-degree of its neighbours. If the result has fewer vertices than the graph, the graph has a cycle.

```csharp
List<int> TopologicalSortKahn(Dictionary<int, List<int>> graph)
{
    var inDegree = new Dictionary<int, int>();
    foreach (var node in graph.Keys) inDegree[node] = 0;
    foreach (var neighbours in graph.Values)
        foreach (var n in neighbours)
            inDegree[n] = inDegree.GetValueOrDefault(n) + 1;

    var queue = new Queue<int>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
    var result = new List<int>();

    while (queue.Count > 0)
    {
        int node = queue.Dequeue();
        result.Add(node);
        foreach (var neighbour in graph.GetValueOrDefault(node, new()))
        {
            if (--inDegree[neighbour] == 0) queue.Enqueue(neighbour);
        }
    }

    if (result.Count != inDegree.Count)
        throw new InvalidOperationException("Graph has a cycle.");
    return result;
}
```

## DFS version

Visit each vertex depth-first, and push it onto a stack after all of its neighbours are visited. Popping the stack gives the topological order.

```csharp
List<int> TopologicalSortDfs(Dictionary<int, List<int>> graph)
{
    var visited = new HashSet<int>();
    var visiting = new HashSet<int>(); // detects cycles
    var stack = new Stack<int>();

    void Visit(int node)
    {
        if (visiting.Contains(node))
            throw new InvalidOperationException("Graph has a cycle.");
        if (visited.Contains(node)) return;

        visiting.Add(node);
        foreach (var neighbour in graph.GetValueOrDefault(node, new()))
            Visit(neighbour);
        visiting.Remove(node);
        visited.Add(node);
        stack.Push(node);
    }

    foreach (var node in graph.Keys) Visit(node);
    return stack.ToList();
}
```

## Complexity and notes

- Time complexity: O(V + E) for both versions. Space: O(V).
- A DAG can have more than one valid topological order. Kahn's algorithm with a queue gives one of them; a priority queue gives the lexicographically smallest.
- Topological sort does not find the shortest path — it only gives a valid ordering. For shortest paths in a DAG, relax edges in topological order (O(V + E)).
- If the input is not a DAG, both versions must detect the cycle instead of returning a wrong order.
