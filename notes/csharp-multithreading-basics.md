# Multithreading Basics in C#

A thread is the smallest unit of execution in a process. Multithreading runs several threads in parallel — across CPU cores where available — so a program can do more than one thing at the same time.

## Key points

- Prefer the Task Parallel Library over raw threads: `Task.Run` schedules work on the thread pool, and `async`/`await` handles I/O-bound work without blocking a thread.
- Use `async`/`await` for waiting on I/O (network, disk, database). Use parallel CPU work (`Task.Run`, `Parallel.For`) for compute-bound work.
- Shared mutable state is the main source of bugs. Protect it with `lock`, `SemaphoreSlim`, `Interlocked`, or thread-safe collections such as `ConcurrentDictionary<TKey, TValue>`.
- A race condition happens when two threads read-modify-write the same variable at the same time; one update can silently overwrite the other.
- A deadlock happens when two threads each hold a lock the other is waiting for. Keep lock scopes small, always acquire locks in the same order, and avoid holding a lock while calling unknown code.
- The thread pool reuses threads, so creating a new `Thread` per unit of work is usually wasteful; `Task` and `Parallel` use the pool for you.

## Example

```csharp
int counter = 0;
object gate = new object();

void Increment()
{
    lock (gate) { counter++; }
}

Parallel.For(0, 1000, _ => Increment());
Console.WriteLine(counter); // 1000
```

Without the `lock`, the result would often be less than 1000, because increments from different threads would overwrite each other.
