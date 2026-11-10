# Singleton Pattern in C#

The Singleton pattern makes sure a class has exactly one instance and gives
global access to it. Use it for things that are naturally single: a
configuration holder, a logger, or a cache manager.

## Thread-safe implementation with `Lazy<T>`

`Lazy<T>` handles thread safety for you, so this is the version to use in
modern C#:

```csharp
public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _instance =
        new Lazy<AppSettings>(() => new AppSettings());

    public static AppSettings Instance => _instance.Value;

    private AppSettings()
    {
        // Private constructor: nobody else can create an instance.
        ApiBaseUrl = "https://api.example.com";
    }

    public string ApiBaseUrl { get; }
}
```

Usage:

```csharp
string url = AppSettings.Instance.ApiBaseUrl;
```

## Why `Lazy<T>` and not double-checked locking

- `Lazy<T>` is thread-safe by default (`LazyThreadSafetyMode.ExecutionAndPublication`).
- The instance is created only on first use, not at application start.
- The code is short and hard to get wrong, unlike hand-written locking.

## When to use a Singleton

- A single shared resource: a connection pool, an in-memory cache, a
  settings object.
- An object that is expensive to create and stateless, or whose state must
  be shared by the whole application.

## When NOT to use a Singleton

- When you need the object in unit tests with different behaviour: a
  Singleton is hard to replace with a fake. Prefer registering the class
  as a singleton in the ASP.NET Core DI container instead — it gives you
  one instance per application, but tests can still substitute it.
- When the "single instance" is really just a global variable in disguise.
  Hidden shared state makes code harder to reason about.
- When the class holds mutable state that different threads change: you
  still need locking inside the Singleton itself.

## Singleton vs static class

A static class cannot implement an interface or be passed as a dependency.
A Singleton is a normal object, so it can implement interfaces and be
injected — that is why it is usually the better choice.
