# Extension Methods in C#

An extension method adds a new method to an existing type, without changing that type's source code or deriving a new class from it.

## Key points

- An extension method is a `static` method in a `static` class, and its first parameter is marked with `this`, for example `public static string Capitalize(this string text)`.
- Call it as if it were an instance method: `"hello".Capitalize()`.
- The extended type can be a class, a struct, an interface, or a generic type, and the method works for every value of that type.
- An extension method can only use the type's public members; it cannot reach private fields or methods.
- If a type later gains a real method with the same signature, the real method wins over the extension method.
- Bring extension methods into scope with a `using` for their namespace; LINQ itself is a set of extension methods on `IEnumerable<T>`.
- Use them to add small, reusable helpers for types you do not own, and keep them in a clearly named static class so they are easy to find.

## Example

```csharp
public static class StringExtensions
{
    public static string Capitalize(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return char.ToUpperInvariant(text[0]) + text[1..];
    }
}

Console.WriteLine("hello".Capitalize()); // Hello
```
