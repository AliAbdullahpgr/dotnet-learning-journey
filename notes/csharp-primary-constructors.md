# Primary Constructors in C# 12 (C#)

A primary constructor lets you declare a class or struct's constructor parameters right in the type declaration. It removes the boilerplate of a separate constructor that only assigns parameters to fields, and it is the same syntax that records have used since C# 9.

## The Basic Idea

Parameters listed after the type name are available throughout the whole class body. The compiler does not create fields for them automatically — you decide what to store and how.

```csharp
public class BankAccount(string owner, decimal balance)
{
    public string Owner { get; } = owner;
    public decimal Balance { get; private set; } = balance;

    public void Deposit(decimal amount) => Balance += amount;

    public override string ToString() => $"{Owner}: {Balance:C}";
}
```

Calling it looks the same as any constructor: `new BankAccount("Daim", 1000m)`.

## Captured Parameters

A primary constructor parameter can be used anywhere in the class body, not just in property initializers. Methods and other members can read it directly — this is called capturing the parameter.

```csharp
public class Greeter(string name)
{
    // name is captured and used here, no field needed.
    public string Greet() => $"Hello, {name}!";
}
```

Be careful with capturing: if a parameter is only used inside a lambda or a local function, the compiler may capture it in a hidden field, which keeps the value alive as long as the instance lives. Storing it in an explicit field makes the lifetime obvious.

## Primary Constructors for Structs

Structs support primary constructors too, but every field must be definitely assigned. A parameterless constructor is still available, so a struct's fields need defaults that work when the primary constructor is not used.

```csharp
public readonly struct Point(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public double DistanceFromOrigin() => Math.Sqrt(X * X + Y * Y);
}
```

## Readonly Fields and Properties

A common pattern is to store a primary constructor parameter in a private readonly field, so it is set once and never changed. This keeps the type immutable in the places that matter while still allowing mutable state elsewhere.

```csharp
public class OrderService(IOrderRepository repository, ILogger<OrderService> logger)
{
    private readonly IOrderRepository _repository = repository;
    private readonly ILogger<OrderService> _logger = logger;

    public async Task<Order?> GetAsync(int id)
    {
        _logger.LogInformation("Loading order {Id}", id);
        return await _repository.GetByIdAsync(id);
    }
}
```

This is the pattern ASP.NET Core uses heavily: inject dependencies through the primary constructor and store them in readonly fields, with no separate constructor body at all.

## When to Use Them — and When Not To

Use a primary constructor when the constructor only stores its parameters, especially for dependency injection and simple immutable types. Prefer a classic constructor when the constructor needs validation logic, when it has multiple overloads, or when the parameter-to-field mapping would confuse a reader — a constructor body makes that logic visible. A class can also mix both: a primary constructor plus additional constructors that chain to it with `: this(...)`. Primary constructors are a readability tool, not a new capability; if the classic version is clearer, keep it.
