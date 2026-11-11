# EF Core Raw SQL Queries

EF Core usually generates the SQL from LINQ, but sometimes hand-written
SQL is the right tool: a complex report, a query LINQ cannot express, or
a stored procedure. EF Core runs raw SQL and still maps the results to
your entities.

## `FromSql` for queries

```csharp
// FromSqlInterpolated: parameters are sent safely, never concatenated.
var cheap = await db.Products
    .FromSqlInterpolated(
        $"SELECT * FROM Products WHERE Price < {100m}")
    .ToListAsync();

// FromSqlRaw: use placeholders and pass the values separately.
var byName = await db.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Name LIKE {0}", "%key%")
    .ToListAsync();
```

Both compose with LINQ — you can add `.Where(...)` or `.OrderBy(...)`
after `FromSql`, and EF Core folds it into one query.

## Key-value types and stored procedures

```csharp
// A stored procedure call is just raw SQL too.
var top = await db.Products
    .FromSqlInterpolated($"EXEC dbo.GetTopProducts @Count = {10}")
    .ToListAsync();

// Non-query commands: inserts, updates, deletes.
int affected = await db.Database.ExecuteSqlInterpolatedAsync(
    $"UPDATE Products SET Price = Price * {1.1m} WHERE CategoryId = {3}");
```

For results that are not an entity, use `SqlQuery<T>` with a simple DTO
or primitive type — for example `db.Database.SqlQuery<int>(...)`.

## Rules to stay safe

- **Never** build SQL by concatenating user input. `FromSqlRaw` with
  string interpolation is an injection hole; always use
  `FromSqlInterpolated` or parameter placeholders.
- Raw SQL bypasses LINQ's compile-time checks, so a renamed column breaks
  at runtime, not at build time — keep raw queries few and tested.
- Entities returned by `FromSql` are tracked like any other query; add
  `.AsNoTracking()` for read-only reports.
- `FromSql` requires the SQL to return **all** columns the entity maps.
  Selecting a subset throws — use a DTO with `SqlQuery<T>` instead.
