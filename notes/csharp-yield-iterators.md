# Yield and Iterators in C#

The `yield` keyword turns an ordinary method into an iterator: it produces one item at a time, only when the caller asks for it, instead of building a whole collection up front.

## Key points

- `yield return value;` hands one item to the caller and pauses the method until the next item is requested.
- `yield break;` ends the sequence early, like a `return` with no value.
- An iterator method returns `IEnumerable<T>` or `IEnumerator<T>`, and the compiler builds the state machine that remembers where it stopped.
- Execution is lazy (deferred): the method body does not run at all until the sequence is enumerated, and it runs again each time it is enumerated.
- This saves memory for large or endless sequences, because only the current item needs to exist at a time.
- `yield return` cannot be used inside a `try` block that has a `catch`, but it can be used in a `try` with only a `finally`.
- Call `.ToList()` or `.ToArray()` when you need the whole sequence materialised at once, for example to enumerate it twice safely.

## Example

```csharp
IEnumerable<int> CountUp(int limit)
{
    for (int i = 1; i <= limit; i++)
    {
        yield return i;
    }
}

foreach (int number in CountUp(3))
{
    Console.WriteLine(number);
}
// Output:
// 1
// 2
// 3
```
