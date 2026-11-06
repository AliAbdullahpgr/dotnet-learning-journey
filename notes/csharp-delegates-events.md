# C# Delegates and Events

## What is a delegate?

A delegate is a type-safe reference to a method. Think of it as a
variable that holds a method instead of a value. The method it points
to must match the delegate's signature (same parameters, same return).

```csharp
public delegate void Notify(string message);

void SendEmail(string message) => Console.WriteLine($"Email: {message}");

Notify handler = SendEmail;
handler("Order placed"); // prints: Email: Order placed
```

## Built-in delegate types

You rarely declare your own delegate today. The framework provides:

- `Action<T>` — takes parameters, returns void.
- `Func<T, TResult>` — takes parameters, returns a value.
- `Predicate<T>` — takes one parameter, returns `bool`.

```csharp
Func<int, int, int> add = (a, b) => a + b;
Action<string> log = msg => Console.WriteLine(msg);
Predicate<int> isEven = n => n % 2 == 0;
```

## Multicast delegates

A delegate can hold a list of methods. `+=` adds a method,
`-=` removes one, and invoking the delegate calls them in order.

```csharp
Notify handlers = SendEmail;
handlers += msg => Console.WriteLine($"SMS: {msg}");
handlers("Hi"); // both methods run
```

## Events

An `event` is a delegate with restricted access: outside code can
subscribe (`+=`) and unsubscribe (`-=`), but only the class that
declares the event can raise it. This is the publisher/subscriber
pattern — the core of UI handlers and notifications.

```csharp
public class OrderService
{
    public event Action<string>? OrderPlaced;

    public void PlaceOrder(string id)
    {
        // ... save the order ...
        OrderPlaced?.Invoke(id); // ?. guards against no subscribers
    }
}

var service = new OrderService();
service.OrderPlaced += id => Console.WriteLine($"Order {id} placed");
service.PlaceOrder("A-101");
```

## Things to remember

- Always null-check before invoking (`?.Invoke(...)`) — an event
  with no subscribers is `null`.
- For thread safety, copy the event to a local variable first, then
  invoke the copy.
- Delegates and events keep the publisher and subscriber loosely
  coupled: the publisher never knows who is listening.
