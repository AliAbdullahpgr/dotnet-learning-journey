# Factory Pattern in C#

The Factory pattern creates objects without the caller knowing the exact
class. Instead of `new` everywhere, a factory method decides which
implementation to return. This keeps creation logic in one place and makes
the calling code easy to extend.

## Simple factory

```csharp
public interface IPaymentMethod
{
    void Pay(decimal amount);
}

public class CardPayment : IPaymentMethod
{
    public void Pay(decimal amount) => Console.WriteLine($"Paid {amount} by card.");
}

public class CashPayment : IPaymentMethod
{
    public void Pay(decimal amount) => Console.WriteLine($"Paid {amount} in cash.");
}

public static class PaymentFactory
{
    public static IPaymentMethod Create(string method) =>
        method.ToLower() switch
        {
            "card" => new CardPayment(),
            "cash" => new CashPayment(),
            _ => throw new ArgumentException("Unknown payment method.", nameof(method)),
        };
}
```

Usage:

```csharp
IPaymentMethod payment = PaymentFactory.Create("card");
payment.Pay(100m);
```

Adding a new payment method means adding one class and one line in the
factory — the checkout code never changes.

## Factory Method

Instead of a static factory, a base class can define a `Create` method
that subclasses override, so each subclass decides which object to make.
ASP.NET Core uses this idea in places like `DbContext` creation with
`IDbContextFactory`.

## When to use a Factory

- Object creation needs logic: choosing between implementations, reading
  configuration, or validating input first.
- The caller should depend on an interface, not a concrete class.
- The same creation code would otherwise be copied in many places.

## When NOT to use a Factory

- The class is simple and `new` is enough. A factory around a plain
  constructor adds complexity for no benefit.
- The created object's type never varies. In that case, direct construction
  (or dependency injection) is clearer.
- Do not confuse a factory with the DI container: in ASP.NET Core,
  `builder.Services.AddTransient<IPaymentMethod, CardPayment>()` already
  acts as the factory for most cases.
