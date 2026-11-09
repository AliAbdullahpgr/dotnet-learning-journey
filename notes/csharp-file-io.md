# File IO Basics in C#

File input/output in C# lives in the `System.IO` namespace. The static `File` class covers most common tasks, while streams give control over large files and custom encodings.

## Key points

- `File.ReadAllText` and `File.WriteAllText` read or write a whole file at once — ideal for small files, wasteful for very large ones.
- `File.ReadAllLines` returns the file as a string array, one element per line; `File.ReadLines` streams lines lazily.
- For large files, use `StreamReader` and `StreamWriter` to process data in chunks instead of loading it all into memory.
- Always wrap streams in a `using` statement (or declaration) so the file handle is released even if an exception occurs.
- `Path.Combine` builds paths with the correct separator on any platform; `Path.GetFileName` and `Path.GetExtension` parse paths safely.
- Common exceptions: `FileNotFoundException`, `IOException`, and `UnauthorizedAccessException` — catch the specific ones you can handle.

## Example

```csharp
string path = Path.Combine("notes", "example.txt");
File.WriteAllText(path, "Hello, file IO!");
string text = File.ReadAllText(path);
Console.WriteLine(text); // Hello, file IO!
```

For async code, use the matching async methods such as `File.ReadAllTextAsync` and `File.WriteAllTextAsync`.
