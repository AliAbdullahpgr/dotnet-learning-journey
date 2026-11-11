# System.Threading.Channels (C#)

A `Channel<T>` is a thread-safe queue built for producer/consumer work:
one side writes items, the other reads them, and the channel handles the
waiting, the backpressure and the shutdown. It replaces hand-rolled
`BlockingCollection` and queue-plus-`SemaphoreSlim` code.

## Creating a channel

```csharp
// Unbounded: writers never block; memory grows if readers fall behind.
var unbounded = Channel.CreateUnbounded<string>();

// Bounded: writers wait when the channel is full — natural backpressure.
var bounded = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait // or DropOldest / DropWrite
});
```

`FullMode` decides what happens when a bounded channel is full: `Wait`
(block the writer), `DropNewest`, `DropOldest`, or `DropWrite`.

## Producer and consumer

```csharp
var channel = Channel.CreateBounded<int>(10);

// Producer — a background service, an API endpoint, a file watcher.
async Task ProduceAsync()
{
    for (int i = 0; i < 100; i++)
        await channel.Writer.WriteAsync(i);
    channel.Writer.Complete(); // no more items — readers can finish
}

// Consumer — reads until the writer completes.
await foreach (int item in channel.Reader.ReadAllAsync())
{
    await ProcessAsync(item);
}
```

`WriteAsync` waits when the channel is full; `ReadAllAsync` ends cleanly
once the writer calls `Complete()`. `TryWrite` / `TryRead` are the
non-blocking variants.

## Where channels fit

- **Background work queues** — an API endpoint enqueues an email or a
  report job; a `BackgroundService` drains the channel. This is the
  standard in-process alternative to a message broker.
- **Pipelines** — chain channels: stage 1 reads raw items and writes
  parsed ones, stage 2 enriches them, stage 3 saves them. Each stage
  runs concurrently and backpressure flows backwards automatically.
- **Throttling producers** — a fast file reader or API poller cannot
  outrun a slow database writer; the bounded channel is the buffer and
  the brake in one.

## Channels vs other options

- vs `BlockingCollection<T>` — channels are async-native (`await` instead
  of blocking a thread) and integrate with `CancellationToken`.
- vs a message broker (RabbitMQ, Azure Service Bus) — channels are
  in-process only. They vanish on restart and do not scale across
  machines; use a broker when work must survive a crash.
- Always `Complete()` the writer and pass a `CancellationToken` to the
  reader, or a shutdown will hang waiting on a channel that never ends.
