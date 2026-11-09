# Stack and Queue

A stack and a queue are both simple collections that control the order in which items are added and removed. The difference is which end comes out first.

## Key points

- A stack is last in, first out (LIFO): `Push` adds to the top, `Pop` removes from the top, and `Peek` looks at the top without removing it.
- A queue is first in, first out (FIFO): `Enqueue` adds to the back, `Dequeue` removes from the front, and `Peek` looks at the front without removing it.
- In C#, use the generic `Stack<T>` and `Queue<T>` types.
- Both give O(1) push/pop and enqueue/dequeue in the usual case, and both grow automatically as items are added.
- A stack suits undo history, the call stack, expression evaluation, and depth-first search.
- A queue suits work that must be handled in arrival order: print jobs, message queues, and breadth-first search.
- Neither is a good fit for random access in the middle; use a `List<T>` or a `LinkedList<T>` for that.
- Popping or dequeuing from an empty collection throws `InvalidOperationException`, so check `Count` first or use `TryPop` / `TryDequeue`.

## Example

```csharp
// A stack reverses the order of the characters.
var stack = new Stack<char>();
foreach (char c in "abc") stack.Push(c);

string reversed = string.Concat(stack); // "cba"

// A queue preserves the order of arrival.
var queue = new Queue<string>();
queue.Enqueue("first");
queue.Enqueue("second");
string next = queue.Dequeue(); // "first"
```
