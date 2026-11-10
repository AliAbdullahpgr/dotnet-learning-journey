# Graph Basics (DSA)

A graph is a set of vertices (nodes) connected by edges. Graphs model
relationships: social networks, road maps, dependency trees, and the web
itself. Unlike a tree, a graph can have cycles, and a node can have any
number of neighbours.

## Terminology

- **Vertex / node** — one item in the graph.
- **Edge** — a connection between two vertices.
- **Directed graph** — edges have a direction (A → B does not imply B → A).
- **Undirected graph** — edges go both ways.
- **Weighted graph** — each edge carries a cost, e.g. distance.
- **Cycle** — a path that starts and ends at the same vertex.
- **Degree** — how many edges touch a vertex.

## Representation: adjacency list

The adjacency list is the usual choice in code: a dictionary from each
vertex to its neighbours. It uses O(V + E) memory, while an adjacency
matrix uses O(V²) and wastes space on sparse graphs.

```csharp
var graph = new Dictionary<int, List<int>>
{
    [1] = new List<int> { 2, 3 },
    [2] = new List<int> { 1, 4 },
    [3] = new List<int> { 1, 4 },
    [4] = new List<int> { 2, 3 },
};
```

## Breadth-first search (BFS)

BFS visits vertices level by level using a queue. It finds the shortest
path (fewest edges) in an unweighted graph.

```csharp
public static List<int> Bfs(Dictionary<int, List<int>> graph, int start)
{
    var visited = new HashSet<int> { start };
    var queue = new Queue<int>();
    var order = new List<int>();
    queue.Enqueue(start);

    while (queue.Count > 0)
    {
        int current = queue.Dequeue();
        order.Add(current);

        foreach (int neighbour in graph[current])
        {
            if (visited.Add(neighbour))
            {
                queue.Enqueue(neighbour);
            }
        }
    }

    return order;
}
```

## Depth-first search (DFS)

DFS goes as deep as possible down one path before backtracking, using a
stack (or recursion). It is the natural fit for detecting cycles and for
topological sorting.

```csharp
public static void Dfs(
    Dictionary<int, List<int>> graph, int current, HashSet<int> visited)
{
    visited.Add(current);
    Console.WriteLine(current);

    foreach (int neighbour in graph[current])
    {
        if (!visited.Contains(neighbour))
        {
            Dfs(graph, neighbour, visited);
        }
    }
}
```

## Complexities

- BFS and DFS both run in O(V + E) time on an adjacency list.
- A `visited` set is essential: without it, a cycle makes the traversal
  loop forever.
- Use BFS when you need the shortest path in an unweighted graph; use DFS
  when you need to explore fully or detect cycles.

## Where graphs appear in .NET work

- Dependency graphs between services or packages.
- Friend/follower relationships in a social feature.
- Route planning and network topology.
- Build systems: topological sort orders tasks so dependencies run first.
