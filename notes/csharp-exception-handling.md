# C# Exception Handling

Exceptions signal that something went wrong at runtime, such as a missing file or a division by zero. Handling them keeps one failure from crashing the whole program.

## try, catch, finally

```csharp
try
{
    int result = 10 / divisor;
    Console.WriteLine(result);
}
catch (DivideByZeroException)
{
    Console.WriteLine("Cannot divide by zero.");
}
catch (Exception ex)
{
    Console.WriteLine($"Something went wrong: {ex.Message}");
}
finally
{
    // Always runs: good for closing a file or connection.
    Console.WriteLine("Cleanup done.");
}
```

## Rules worth remembering

- Catch the most specific exception first. A general `catch (Exception)` placed first would swallow every later, specific catch.
- Use `throw;` to rethrow inside a catch block. `throw ex;` resets the stack trace and hides where the error started.
- Only catch exceptions you can actually handle. Let the rest bubble up rather than hiding them with an empty catch block.
