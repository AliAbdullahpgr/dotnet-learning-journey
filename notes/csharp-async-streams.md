# Async Streams (IAsyncEnumerable<T>) in C#

`IAsyncEnumerable<T>` is a stream: items arrive one at a time, each
produced asynchronously, and the caller processes each item as soon as
it arrives instead of waiting for a whole list. It is the async twin
of `IEnumerable<T>`.

## The shape

```csharp
// Produces numbers slowly, one at a time.
async IAsyncEnumerable<int> CountSlowlyAsync()
{
    for (int i = 1; i <= 3; i++)
    {
        await Task.Delay(500); // e.g. reading a row, calling an API
        yield return i;        // hand this item to the caller now
    }
}

// Consume with await foreach — each item is handled as it arrives.
await foreach (int number in CountSlowlyAsync())
{
    Console.WriteLine(number);
}
```

`yield return` inside an `async` method is what makes the method a
stream: the method pauses after each item and resumes when the caller
asks for the next one.

## Where streams beat lists

- **Database rows** — `await foreach (var row in db.Rows.AsAsyncEnumerable())`
  processes rows while the query is still running, instead of loading
  them all into memory first.
- **Paging an API** — fetch page 1, yield its items, then fetch page 2
  only if the caller keeps reading. The caller can stop early and the
  later pages are never fetched.
- **Server-sent events / SignalR** — push items to the client as they
  are produced.
- If the caller needs everything at once (sorting, counting), a stream
  buys nothing — just return `Task<List<T>>`.

## Cancellation

Pass the token through with the `[EnumeratorCancellation]` attribute,
so the caller's token actually cancels the producer:

```csharp
async IAsyncEnumerable<string> ReadLinesAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (string line in File.ReadLinesAsync("data.txt", cancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        yield return line;
    }
}

await foreach (string line in ReadLinesAsync(cts.Token))
{
    Console.WriteLine(line);
}
```

Without the attribute, the token passed to `await foreach` would be
ignored by the producer.

## Rules and pitfalls

- `await foreach` needs `await using` for disposables: an async
  stream that owns a `DbDataReader` should be consumed inside
  `await using var enumerator = ...` or the reader may leak.
- Exceptions surface during enumeration, not when the method is
  called — wrap the `await foreach` in try/catch.
- LINQ over async streams needs `System.Linq.Async` (the
  `System.Linq.Async` package): `WhereAwait`, `SelectAwait`,
  `ToListAsync()`. Plain LINQ operators do not exist for
  `IAsyncEnumerable<T>`.

## Takeaway

Use `IAsyncEnumerable<T>` when items are produced slowly or in large
numbers and the caller can work on them one at a time. It keeps memory
flat and lets the caller stop early — but always wire cancellation
through, and remember the work (and the exceptions) happen during
enumeration, not at the call.
