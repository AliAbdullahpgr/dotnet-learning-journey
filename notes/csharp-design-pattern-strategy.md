# Strategy Pattern in C#

The Strategy pattern lets you swap an algorithm at runtime by putting each
algorithm in its own class behind a shared interface. The code that uses the
algorithm never knows which one is running — it just calls the interface.

## The pattern

```csharp
// The strategy interface: one method, one job.
public interface IShippingCost
{
    decimal Calculate(decimal weightKg);
}

public class StandardShipping : IShippingCost
{
    public decimal Calculate(decimal weightKg) => 5m + weightKg * 1m;
}

public class ExpressShipping : IShippingCost
{
    public decimal Calculate(decimal weightKg) => 15m + weightKg * 3m;
}

public class FreeShipping : IShippingCost
{
    public decimal Calculate(decimal weightKg) => 0m;
}

// The context: uses whatever strategy it was given.
public class Order
{
    private readonly IShippingCost _shipping;

    public Order(IShippingCost shipping) => _shipping = shipping;

    public decimal Total(decimal itemsPrice, decimal weightKg) =>
        itemsPrice + _shipping.Calculate(weightKg);
}
```

Usage:

```csharp
var standard = new Order(new StandardShipping());
Console.WriteLine(standard.Total(100m, 2m)); // 107

var express = new Order(new ExpressShipping());
Console.WriteLine(express.Total(100m, 2m)); // 121
```

## Why it beats `if/else` chains

- Each algorithm lives in its own class, so it can be tested alone.
- Adding a new algorithm means adding one class — the `Order` class is
  never edited (Open/Closed Principle).
- Strategies can be picked from configuration, user choice or A/B tests,
  without touching the calling code.

## Where it shows up in .NET

- ASP.NET Core authentication schemes: each scheme is a strategy for
  validating a request.
- Sorting: `IComparer<T>` is a strategy for ordering.
- Compression, serialization and password hashing all swap algorithms
  behind an interface.

## Strategy vs Factory

A Factory decides *which object to create*; a Strategy decides *which
algorithm to run*. They often appear together: a factory picks the
strategy, and the context runs it.
