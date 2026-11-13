# Garbage Collection in .NET (C#)

The .NET garbage collector (GC) automatically frees memory that your program no longer uses. You rarely manage memory by hand, but understanding generations, the large object heap, and `IDisposable` keeps your apps from leaking memory or pausing at the wrong time.

## Generations 0, 1, and 2

The GC divides the managed heap into three generations, based on a simple bet: most objects die young.

- **Gen 0** — new objects. Collected most often and very cheaply; most objects are already dead by the time it runs.
- **Gen 1** — objects that survived one Gen 0 collection. A buffer between short-lived and long-lived objects.
- **Gen 2** — long-lived objects (caches, singletons, static data). Collected rarely, and a full Gen 2 collection is the most expensive one.

```csharp
var list = new List<int>();
Console.WriteLine(GC.GetGeneration(list)); // 0 — just created

GC.Collect(0);                            // force a Gen 0 collection
Console.WriteLine(GC.GetGeneration(list)); // 1 — it survived
```

## The large object heap (LOH)

Objects of 85,000 bytes or more go to the large object heap instead of Gen 0. The LOH is only collected during Gen 2 collections, and historically it was not compacted, so repeatedly allocating and freeing large arrays (for example, big byte buffers) fragments it.

```csharp
byte[] big = new byte[100_000]; // lands on the LOH
```

Prefer reusing buffers (`ArrayPool<byte>.Shared.Rent(100_000)`) over allocating large arrays in a loop. `ArrayPool` returns the buffer to a shared pool instead of leaving garbage on the LOH.

## IDisposable and using

The GC frees managed memory, but it does not close files, database connections, or network sockets. Types that hold unmanaged resources implement `IDisposable`, and you release them with `using`.

```csharp
using var stream = File.OpenRead("data.txt");
using var reader = new StreamReader(stream);
string text = await reader.ReadToEndAsync();
// Dispose is called automatically at the end of the block, even on exception
```

Forgetting to dispose a `SqlConnection` or a `FileStream` leaks the underlying handle until the finalizer runs — or forever, if the process ends first.

## Finalizers

A finalizer (`~MyClass()`) is a last-resort cleanup that runs when the GC collects the object. It runs on a dedicated thread, at an unpredictable time, and an object with a finalizer survives one extra collection — so it is slower to free.

```csharp
public class NativeResource : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;
        ReleaseHandle(_handle);   // free the unmanaged resource now
        _disposed = true;
        GC.SuppressFinalize(this); // no need for the finalizer anymore
    }

    ~NativeResource() => ReleaseHandle(_handle); // safety net only
}
```

Rule of thumb: implement a finalizer only when you hold a raw unmanaged handle directly. If you only wrap other `IDisposable` objects, `Dispose` alone is enough.

## GC.Collect guidance

- **Do not call `GC.Collect()` in normal code.** The GC tunes itself to your app's allocation pattern; forcing a collection usually makes things worse.
- The one legitimate use is a known quiet moment after a huge, one-off allocation spike (for example, after loading a large file into memory and releasing it).
- If you measure memory pressure, watch Gen 2 collection counts and LOH size — frequent Gen 2 collections mean too many long-lived objects are churning, which is a design problem, not a GC setting.
