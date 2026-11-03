# C# Records

A record is a reference type designed for immutable data. Unlike a class, a
record gives you value-based equality: two records are equal when their
property values are equal, not only when they are the same object.

```csharp
public record Person(string FirstName, string LastName);

var a = new Person("Daim", "Ali");
var b = new Person("Daim", "Ali");
Console.WriteLine(a == b); // True — value equality
```

## Positional syntax

The primary constructor in the declaration above creates init-only
properties automatically. You can also declare a record with a body and add
extra members, just like a class.

## Non-destructive mutation with `with`

Records are immutable, so you never change an instance in place. Instead,
`with` creates a copy with only the listed properties changed:

```csharp
var updated = a with { LastName = "Khan" };
// a is unchanged; updated is a new Person("Daim", "Khan")
```

## When to use a record

- Data transfer objects (DTOs) and API models
- Values you compare by content, e.g. keys or messages
- Any small immutable model where equality should mean "same data"

Use a class instead when the object has changing state, identity that
matters (e.g. a database entity tracked by EF Core), or behaviour that
mutates internal fields.
