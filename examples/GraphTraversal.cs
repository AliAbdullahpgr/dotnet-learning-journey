// GraphTraversal.cs
// Builds a small adjacency-list graph and prints BFS and DFS visit orders.
// Run with: dotnet run (inside a console project) or dotnet script.

using System;
using System.Collections.Generic;

class Program
{
    // Adjacency list: each vertex maps to the list of its neighbours.
    static Dictionary<string, List<string>> BuildGraph() => new()
    {
        ["A"] = new() { "B", "C" },
        ["B"] = new() { "D", "E" },
        ["C"] = new() { "F" },
        ["D"] = new() { },
        ["E"] = new() { "F" },
        ["F"] = new() { }
    };

    // Breadth-first search: visits level by level using a queue.
    static List<string> Bfs(Dictionary<string, List<string>> graph, string start)
    {
        var visited = new HashSet<string> { start };
        var order = new List<string>();
        var queue = new Queue<string>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            string node = queue.Dequeue();
            order.Add(node);
            foreach (string neighbour in graph[node])
            {
                if (visited.Add(neighbour))
                    queue.Enqueue(neighbour);
            }
        }
        return order;
    }

    // Depth-first search: goes as deep as possible first, using a stack.
    static List<string> Dfs(Dictionary<string, List<string>> graph, string start)
    {
        var visited = new HashSet<string>();
        var order = new List<string>();
        var stack = new Stack<string>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            string node = stack.Pop();
            if (!visited.Add(node))
                continue;

            order.Add(node);
            // Push neighbours in reverse so the first neighbour is visited first.
            for (int i = graph[node].Count - 1; i >= 0; i--)
                stack.Push(graph[node][i]);
        }
        return order;
    }

    static void Main()
    {
        var graph = BuildGraph();

        // Graph:
        //       A
        //      / \
        //     B   C
        //    / \   \
        //   D   E - F
        Console.WriteLine("BFS from A: " + string.Join(" -> ", Bfs(graph, "A")));
        Console.WriteLine("DFS from A: " + string.Join(" -> ", Dfs(graph, "A")));
    }
}

// Expected output:
// BFS from A: A -> B -> C -> D -> E -> F
// DFS from A: A -> B -> D -> E -> F -> C
