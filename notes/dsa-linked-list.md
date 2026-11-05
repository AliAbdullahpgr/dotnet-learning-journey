# Linked Lists (DSA)

A linked list is a linear data structure where each element (node) stores a value and a reference to the next node. Unlike an array, nodes do not sit in contiguous memory, so the list can grow and shrink without copying.

## Singly linked list in C#

```csharp
public class Node
{
    public int Value { get; set; }
    public Node? Next { get; set; }

    public Node(int value) => Value = value;
}

public class LinkedList
{
    private Node? _head;

    // Insert at the head: O(1)
    public void AddFirst(int value)
    {
        var node = new Node(value) { Next = _head };
        _head = node;
    }

    // Insert at the tail: O(n) — must walk to the end
    public void AddLast(int value)
    {
        var node = new Node(value);
        if (_head is null)
        {
            _head = node;
            return;
        }

        var current = _head;
        while (current.Next is not null)
            current = current.Next;
        current.Next = node;
    }

    // Search: O(n)
    public bool Contains(int value)
    {
        var current = _head;
        while (current is not null)
        {
            if (current.Value == value) return true;
            current = current.Next;
        }
        return false;
    }

    public void Print()
    {
        var current = _head;
        while (current is not null)
        {
            Console.Write($"{current.Value} -> ");
            current = current.Next;
        }
        Console.WriteLine("null");
    }
}
```

## Time complexity

| Operation | Linked list | Array / `List<T>` |
| --- | --- | --- |
| Insert at head | O(1) | O(n) |
| Insert at tail | O(n) without a tail pointer, O(1) with one | O(1) amortised |
| Access by index | O(n) | O(1) |
| Search | O(n) | O(n) |
| Delete a known node | O(1) | O(n) |

## When to use one

- Frequent insertions/deletions at the front, or in the middle once you already hold the node.
- The size is unknown and changes often, and you do not need random access.
- For most C# work, `List<T>` or `LinkedList<T>` from the BCL is the right default; hand-rolling a list is mainly a learning exercise (and an interview staple).

## Key points

- A **doubly** linked list adds a `Previous` pointer: deletions are easier, at the cost of extra memory per node.
- A **circular** linked list links the last node back to the head — useful for round-robin scheduling.
- The classic cycle check is Floyd's tortoise-and-hare: a slow and a fast pointer; if they meet, the list has a cycle.

*Learning log — 5 Nov 2026*
