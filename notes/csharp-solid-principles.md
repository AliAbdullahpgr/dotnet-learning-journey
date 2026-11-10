# SOLID Principles in C#

SOLID is five design principles for writing code that is easy to change,
test and extend. The letters stand for Single Responsibility, Open/Closed,
Liskov Substitution, Interface Segregation and Dependency Inversion.

## S — Single Responsibility Principle

A class should have one reason to change. Split saving, formatting and
sending into separate classes instead of one class that does everything.

```csharp
// Bad: one class does persistence, formatting and email.
public class ReportService
{
    public string Format(Report r) => "...";
    public void Save(Report r) { /* writes to disk */ }
    public void Email(Report r) { /* sends mail */ }
}

// Better: one job per class.
public class ReportFormatter { public string Format(Report r) => "..."; }
public class ReportRepository { public void Save(Report r) { } }
public class ReportMailer { public void Email(Report r) { } }
```

## O — Open/Closed Principle

Software entities should be open for extension but closed for modification.
Add new behaviour by adding new code, not by editing working code.

```csharp
public interface IDiscount { decimal Apply(decimal price); }

public class StudentDiscount : IDiscount
{
    public decimal Apply(decimal price) => price * 0.9m;
}

// A new discount type is a new class — the checkout code never changes.
public class Checkout
{
    public decimal Total(decimal price, IDiscount discount) =>
        discount.Apply(price);
}
```

## L — Liskov Substitution Principle

A derived class must be usable anywhere its base class is expected, without
breaking the caller. If a subclass throws for an operation the base class
promises, the hierarchy is wrong.

```csharp
// A ReadOnlyFileStream that throws on Write() violates LSP:
// code written against Stream expects Write() to work.
public abstract class StreamBase
{
    public abstract byte[] Read();
    public abstract void Write(byte[] data);
}
```

Fix it by splitting the contract: a readable stream interface and a
separate writable stream interface, so each class only promises what it
can do.

## I — Interface Segregation Principle

Many small, specific interfaces are better than one big interface that
forces classes to implement methods they never use.

```csharp
// Bad: every worker must implement Eat() and Sleep() too.
public interface IWorker { void Work(); void Eat(); void Sleep(); }

// Better: split by capability.
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }

public class Robot : IWorkable
{
    public void Work() { /* robots do not eat */ }
}
```

## D — Dependency Inversion Principle

High-level modules should not depend on low-level modules; both should
depend on abstractions. In ASP.NET Core this is exactly what the built-in
dependency injection container does.

```csharp
public interface INotifier { void Notify(string message); }

public class OrderService
{
    private readonly INotifier _notifier;
    public OrderService(INotifier notifier) => _notifier = notifier;
}
```

`OrderService` depends on the `INotifier` abstraction, so the real sender
(email, SMS, push) can be swapped without touching the service.

## How to use SOLID

- Do not force all five principles into every class. Small scripts and
  throwaway code do not need them.
- The principles pay off most in code that changes often: services,
  domain logic and anything covered by tests.
- When a class is hard to test, it usually violates S or D first — that is
  the best place to start applying SOLID.
