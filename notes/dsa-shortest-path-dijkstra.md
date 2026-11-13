# Dijkstra's Shortest Path Algorithm (DSA)

Dijkstra's algorithm finds the shortest path from a source vertex to every other vertex in a weighted graph. It works only when all edge weights are non-negative.

## How it works

Keep a distances table, initially infinity for every vertex except the source (0). Repeatedly pick the unvisited vertex with the smallest known distance, mark it visited, and relax its outgoing edges: if going through this vertex gives a shorter distance to a neighbour, update the neighbour's distance. A min-heap (priority queue) makes picking the next vertex fast.

## Why non-negative weights matter

Once a vertex is marked visited, its distance is final — Dijkstra never revisits it. A negative edge could later offer a shorter route through a visited vertex, which the algorithm would miss. For graphs with negative weights, use Bellman-Ford instead.

## C# implementation with PriorityQueue

```csharp
Dictionary<string, int> Dijkstra(
    Dictionary<string, List<(string To, int Weight)>> graph, string source)
{
    var distances = new Dictionary<string, int>();
    foreach (var node in graph.Keys) distances[node] = int.MaxValue;
    distances[source] = 0;

    var pq = new PriorityQueue<string, int>();
    pq.Enqueue(source, 0);

    while (pq.Count > 0)
    {
        pq.TryDequeue(out string node, out int dist);
        if (dist > distances[node]) continue; // stale entry, skip

        foreach (var (to, weight) in graph[node])
        {
            int newDist = dist + weight;
            if (newDist < distances[to])
            {
                distances[to] = newDist;
                pq.Enqueue(to, newDist);
            }
        }
    }
    return distances;
}

// Example graph: A -> B (4), A -> C (2), C -> B (1), B -> D (5), C -> D (8)
var graph = new Dictionary<string, List<(string, int)>>
{
    ["A"] = new() { ("B", 4), ("C", 2) },
    ["B"] = new() { ("D", 5) },
    ["C"] = new() { ("B", 1), ("D", 8) },
    ["D"] = new() { }
};

var distances = Dijkstra(graph, "A");
// A = 0, C = 2, B = 3 (via C), D = 8 (via B)
foreach (var kv in distances) Console.WriteLine($"{kv.Key}: {kv.Value}");
```

## The distances table

| Vertex | Shortest distance from A | Path |
|--------|--------------------------|------|
| A | 0 | A |
| C | 2 | A → C |
| B | 3 | A → C → B |
| D | 8 | A → C → B → D |

Note that A → B directly costs 4, but A → C → B costs 3 — the algorithm finds the cheaper route.

## Complexity and notes

- Time complexity with a binary heap: O((V + E) log V). Space: O(V).
- The `if (dist > distances[node]) continue;` check skips stale queue entries — a vertex can be enqueued several times with improving distances.
- To recover the actual path (not just the distance), keep a `previous` dictionary that records which vertex relaxed each neighbour.
- Real uses: GPS routing, network packet routing, and game pathfinding on weighted grids.
