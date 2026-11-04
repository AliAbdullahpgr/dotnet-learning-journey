# C# Pattern Matching

Pattern matching lets code test a value's shape and extract data in one step, instead of nested ifs and casts.

## Type pattern with declaration

```csharp
object value = 42;
if (value is int number)
{
    Console.WriteLine(number * 2); // 84
}
```

## Switch expression

```csharp
string Describe(object value) => value switch
{
    int n when n < 0 => "negative number",
    int => "non-negative number",
    string s => $"text of length {s.Length}",
    null => "nothing",
    _ => "something else"
};
```

## Property pattern

```csharp
record Point(int X, int Y);
bool IsOrigin(Point p) => p is { X: 0, Y: 0 };
```

## Relational pattern

```csharp
string Grade(int score) => score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 60 => "C",
    _ => "F"
};
```

## Key points

- `is` with a declaration both tests and binds a variable.
- Switch expressions must be exhaustive; the `_` discard pattern covers the rest.
- Property patterns destructure without writing a separate cast or null check.
- Prefer patterns when the logic is about the shape of data, not just one boolean flag.
