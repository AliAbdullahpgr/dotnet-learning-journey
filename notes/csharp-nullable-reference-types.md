# Nullable Reference Types in C#

Nullable reference types let the compiler warn you when a reference might be null, so many `NullReferenceException` bugs are caught before the program runs.

## Key points

- Turn the feature on with `<Nullable>enable</Nullable>` in the project file, or with `#nullable enable` in a single file.
- A plain `string` means the value should not be null; `string?` means it is allowed to be null.
- These are compile-time warnings, not runtime checks: the annotation does not stop a null from arriving from outside code.
- Check before use with `is null`, `is not null`, the `?.` operator, or the `??` operator.
- Use the null-forgiving operator `!` only when you know a value is not null, for example `name!.Length`; it silences the warning without adding a check.
- Annotate method parameters and return types too, so callers can see which values may be null.
- Turn the feature on in new projects from the start; adding it to an old project means fixing the warnings one file at a time.

## Example

```csharp
string? GetNickname() => null;

string? nickname = GetNickname();
string greeting = nickname is not null
    ? $"Hello, {nickname}!"
    : "Hello, guest!";

Console.WriteLine(greeting);
// Output: Hello, guest!
```
