# Generics Constraints in C#

A constraint limits which types can be used as a type argument for a generic type or method. It is declared with the `where` clause, and it lets the code safely use members of the constrained type.

## Key points

- `where T : class` — the type argument must be a reference type.
- `where T : struct` — the type argument must be a non-nullable value type.
- `where T : notnull` — the type argument must not be nullable.
- `where T : new()` — the type argument must have a public parameterless constructor, so `new T()` works inside the generic code.
- `where T : SomeBase` or `where T : ISomeInterface` — the type argument must derive from the base class or implement the interface.
- Several constraints can be combined with commas; when `new()` is used, it must come last.
- Constraints are checked at compile time, so using a type argument that breaks one is a compiler error, not a runtime surprise.

## Example

```csharp
public T Max<T>(T first, T second) where T : IComparable<T>
{
    return first.CompareTo(second) >= 0 ? first : second;
}

int bigger = Max(3, 7);          // 7
string later = Max("apple", "pear"); // "pear"
```

The `IComparable<T>` constraint guarantees that `CompareTo` exists, so the method works for any comparable type without casts or runtime checks.
