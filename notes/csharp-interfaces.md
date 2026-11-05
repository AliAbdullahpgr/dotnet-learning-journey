# C# Interfaces

An interface is a contract: it declares members (methods, properties, events, indexers) that a class or struct must implement, without providing the implementation itself (except default interface methods, C# 8+).

## Why use interfaces?

- **Decoupling** — code depends on an abstraction, not a concrete class, so implementations can be swapped (e.g. in tests or DI).
- **Multiple behaviours** — a class can implement many interfaces, even though it inherits from only one base class.
- **Polymorphism** — different types can be used through the same interface.

## Example

```csharp
public interface IPaymentMethod
{
    string Name { get; }
    bool Charge(decimal amount);
}

public class CardPayment : IPaymentMethod
{
    public string Name => "Card";

    public bool Charge(decimal amount)
    {
        // Call the card gateway here.
        return amount > 0;
    }
}

public class CashPayment : IPaymentMethod
{
    public string Name => "Cash";

    public bool Charge(decimal amount) => amount > 0;
}

public class CheckoutService
{
    // Depends on the interface, not on a concrete payment type.
    public void Pay(IPaymentMethod method, decimal amount)
    {
        if (!method.Charge(amount))
            throw new InvalidOperationException("Payment failed.");
    }
}
```

## Interface vs abstract class

| | Interface | Abstract class |
| --- | --- | --- |
| Multiple inheritance | Yes, a class can implement many | No, one base class only |
| State (fields) | No instance fields | Yes |
| Default implementation | Yes, since C# 8 | Yes |
| Best for | A capability a type *can do* | A shared identity a type *is* |

## Key points

- Interface members are `public` by default; no access modifier is needed (or allowed, before C# 8 default members).
- A class must implement **all** interface members, or be declared `abstract` itself.
- Interfaces can inherit from other interfaces.
- Common .NET interfaces: `IEnumerable<T>`, `IDisposable`, `IComparable<T>`, `IEqualityComparer<T>`.
- Prefer depending on small, focused interfaces (Interface Segregation Principle) over one large interface.

*Learning log — 5 Nov 2026*
