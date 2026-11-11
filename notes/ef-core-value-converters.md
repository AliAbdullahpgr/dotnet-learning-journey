# EF Core Value Converters

A value converter translates between the type in your entity and the
type in the database column. Use it when the model wants one shape and
the column wants another — an enum stored as text, a list stored as a
single JSON string, or a value object stored as a plain string.

## Enums as strings

By default EF Core stores an enum as its number, which is unreadable in
raw SQL. Store the name instead:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Order>()
        .Property(o => o.Status)
        .HasConversion<string>(); // Pending, Shipped, Delivered — not 0, 1, 2
}
```

The database column becomes `nvarchar`, and queries on `Status` still
work — EF Core converts the value both ways.

## Custom conversions

```csharp
// A list of tags stored as one comma-separated column.
modelBuilder.Entity<Product>()
    .Property(p => p.Tags)
    .HasConversion(
        tags => string.Join(',', tags),
        text => text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToList());

// Money as a value object, stored as a decimal column.
modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasConversion(
        money => money.Amount,
        amount => new Money(amount, "PKR"));
```

Provide both directions: model → database and database → model. The
second direction runs on every read, so keep it cheap.

## Built-in converters

EF Core ships converters for common pairs, usable with a single generic
argument:

- `.HasConversion<string>()` — enums to their names, `Guid` to text.
- `.HasConversion<int>()` — enums to numbers (the default behaviour).
- `.HasConversion<DateOnly>()` / time-only types map to `date` / `time`
  columns without a custom converter in EF Core 7+.

## Limits to know

- Converted values cannot be used in every query: a `Where` on a
  converted property works, but sorting or grouping by a custom
  conversion may fail to translate — EF Core can only translate what the
  provider understands.
- A converter hides the real shape from the database: a JSON-stored list
  cannot be queried with SQL `LIKE` on individual tags, and indexes on
  the column index the whole string, not the parts.
- For a value object with several fields, prefer an **owned entity**
  (`.OwnsOne(...)`) over a converter — owned types map each field to its
  own column and stay queryable.
