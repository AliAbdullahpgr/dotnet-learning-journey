# Cancellation Tokens in C# (C#)

A cancellation token is the standard way to ask an asynchronous operation to stop. The operation is not killed — it is politely asked to stop, and it checks the token at safe points and throws `OperationCanceledException` when cancellation is requested. This cooperative design keeps resources clean and avoids half-finished work.

## CancellationTokenSource and CancellationToken

A `CancellationTokenSource` creates and controls the token. Calling `Cancel()` signals every token created from that source. The token itself is a lightweight struct that is passed around, never the source.

```csharp
using var cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

// Cancel after 5 seconds, or call cts.Cancel() from elsewhere.
cts.CancelAfter(TimeSpan.FromSeconds(5));

await LongRunningWorkAsync(token);
```

## Passing Tokens to Async Methods

Library methods that support cancellation take the token as their last parameter. Always pass the token through instead of ignoring it — an operation that cannot be cancelled will keep running after the user has given up.

```csharp
public async Task<string> DownloadAsync(string url, CancellationToken token)
{
    using var client = new HttpClient();
    // The token cancels the HTTP request itself.
    return await client.GetStringAsync(url, token);
}

public async Task WaitAsync(CancellationToken token)
{
    // Task.Delay respects the token instead of sleeping blindly.
    await Task.Delay(TimeSpan.FromSeconds(30), token);
}
```

## Checking the Token Yourself

In your own loops and long-running work, check the token regularly with `ThrowIfCancellationRequested()`. Without a check, the token has no effect on your code.

```csharp
public async Task ProcessItemsAsync(IEnumerable<int> items, CancellationToken token)
{
    foreach (var item in items)
    {
        token.ThrowIfCancellationRequested();
        await ProcessOneAsync(item, token);
    }
}
```

## Linked Tokens

`CreateLinkedTokenSource` combines several tokens into one. The linked token is cancelled when any of its sources is cancelled, which is useful when a method has its own timeout but must also respect the caller's token.

```csharp
public async Task WorkAsync(CancellationToken callerToken)
{
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
    using var linked = CancellationTokenSource.CreateLinkedTokenSource(
        callerToken, timeout.Token);

    await DownloadAsync("https://example.com/data", linked.Token);
}
```

## Handling OperationCanceledException

When cancellation happens, the awaiting code receives an `OperationCanceledException` (or `TaskCanceledException`, which derives from it). Catch it only where you need to clean up or report the outcome — do not treat it as an error to log as a failure, because cancellation is a normal result.

```csharp
try
{
    await DownloadAsync("https://example.com/data", token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Download was cancelled.");
}
```

## Rules Worth Remembering

Pass the token to every async call that accepts one, including `HttpClient`, `Task.Delay`, and EF Core's `ToListAsync(token)`. Dispose the `CancellationTokenSource` when you are done with it. Never reuse a cancelled source — create a fresh one for each operation. And remember that cancellation is cooperative: code that never checks the token and never awaits a cancellable operation will simply ignore it.
