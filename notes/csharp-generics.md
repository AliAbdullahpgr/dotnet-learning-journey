# C# Generics

Generics let you write a class, method, or interface once and reuse it with different types, without giving up type safety. Instead of writing separate versions for `int`, `string`, and other types — or falling back to `object` and casting — you write a type parameter, usually called `T`, and let the caller specify the real type.

## Why use generics?

- **Type safety:** The compiler checks the type, so a `List<int>` cannot accidentally hold a `string`.
- **No boxing or casting:** Value types stay as themselves, which avoids the cost and the runtime errors of casting from `object`.
- **Less duplication:** One implementation serves many types.

## A generic class

```csharp
public class Box<T>
{
    public T Value { get; set; }

    public Box(T value)
    {
        Value = value;
    }
}

var intBox = new Box<int>(42);
var textBox = new Box<string>("hello");
```

Here, `T` is a placeholder. When you create `Box<int>`, every `T` in the class becomes `int`.

## A generic method

```csharp
public static void Swap<T>(ref T first, ref T second)
{
    T temp = first;
    first = second;
    second = temp;
}

int a = 1, b = 2;
Swap(ref a, ref b); // a is now 2, b is now 1
```

The compiler can often infer `T` from the arguments, so you do not always have to write it out.

## Constraints

Constraints limit which types can be used as `T`, so the code inside can rely on certain abilities:

```csharp
public class Repository<T> where T : class, new()
{
    public T Create() => new T();
}
```

Common constraints include `where T : class` (a reference type), `where T : struct` (a value type), `where T : new()` (a parameterless constructor), and `where T : SomeBaseClass` or `where T : ISomeInterface`.

## Generics in everyday .NET code

You already use generics constantly: `List<T>`, `Dictionary<TKey, TValue>`, `Nullable<T>`, and `Task<T>` are all generic types. The collections in `System.Collections.Generic` are the best example of why generics matter — a `List<string>` behaves exactly like a list of strings, with no casting.

## Key takeaway

Use generics when the same logic should work for different types. They keep code reusable, type-safe, and free of unnecessary casts.
