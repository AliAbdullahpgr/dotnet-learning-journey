# EF Core Concurrency Tokens (EF Core)

Optimistic concurrency assumes conflicts are rare, so it does not lock rows while a user edits them. Instead, EF Core checks at save time that the row has not changed since it was read. If it has, the save fails with a `DbUpdateConcurrencyException` and the application decides what to do — retry, reload, or tell the user.

## The Problem

Two users load the same product, both change the price, and both save. Without a check, the second save silently overwrites the first. This is the "last writer wins" problem, and it loses data without any error.

## [ConcurrencyCheck]

Marking a property with `[ConcurrencyCheck]` adds it to the `WHERE` clause of the `UPDATE` statement. If the value in the database no longer matches the value that was read, no rows are affected and EF Core throws.

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public decimal Price { get; set; }
}
```

The downside is that every checked column is compared, so a wide check makes the `WHERE` clause long and a harmless change to an unrelated column can still trigger a conflict.

## RowVersion — The Preferred Token

A `byte[]` property marked with `[Timestamp]` (or configured with `IsRowVersion()` in the fluent API) maps to SQL Server's `rowversion` column. The database changes the value on every update, so EF Core only needs to compare one column.

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
```

```csharp
// Fluent API equivalent, no attribute needed.
modelBuilder.Entity<Product>()
    .Property(p => p.RowVersion)
    .IsRowVersion();
```

## Handling DbUpdateConcurrencyException

When the exception is thrown, the entry's original values no longer match the database. A common pattern is to reload the current database values, reapply the user's intended change, and save again — but only a limited number of times.

```csharp
public async Task UpdatePriceAsync(int id, decimal newPrice)
{
    const int maxRetries = 3;

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        try
        {
            var product = await context.Products.FindAsync(id);
            if (product is null) return;

            product.Price = newPrice;
            await context.SaveChangesAsync();
            return;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Reload the current database values so the next
            // attempt compares against the latest row version.
            foreach (var entry in ex.Entries)
                await entry.ReloadAsync();
        }
    }

    throw new InvalidOperationException(
        "The product was changed too many times. Please try again.");
}
```

## When to Use Each Approach

Use a `RowVersion` column for any entity that users edit concurrently — it is cheap, exact, and catches every change. Use `[ConcurrencyCheck]` only when a specific business rule matters, such as "the price must not have changed since the user saw it," and a change to another column should not block the save. Never silently swallow the exception: a failed save means someone else's change would have been lost, and the user deserves to know.
