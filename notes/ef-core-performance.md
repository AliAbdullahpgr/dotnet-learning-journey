# EF Core Performance

Entity Framework Core is fast enough for most apps, but a few habits turn
a quick query into a slow one. These are the highest-impact fixes, in the
order to try them.

## 1. Read without tracking

Queries that only display data do not need change tracking. `AsNoTracking`
skips the snapshot EF Core keeps for every entity:

```csharp
var products = await _db.Products
    .AsNoTracking()
    .Where(p => p.Price < 100)
    .ToListAsync();
```

## 2. Select only what you need

Fetching whole entities when the page shows two columns wastes bandwidth
and memory. Project into a small DTO:

```csharp
var summaries = await _db.Products
    .AsNoTracking()
    .Select(p => new ProductSummary(p.Id, p.Name, p.Price))
    .ToListAsync();
```

## 3. Kill the N+1 problem

Loading a list and then touching a navigation property per row fires one
extra query per row. Eager-load what you need, once:

```csharp
// Bad: 1 query for orders + N queries for customers.
var orders = await _db.Orders.ToListAsync();
foreach (var order in orders) Console.WriteLine(order.Customer.Name);

// Good: one query with a JOIN.
var orders = await _db.Orders
    .Include(o => o.Customer)
    .ToListAsync();
```

## 4. Split big queries

One query with several `Include` calls produces a huge result set with
repeated data (cartesian explosion). `AsSplitQuery` runs one query per
collection instead:

```csharp
var orders = await _db.Orders
    .Include(o => o.Items)
    .Include(o => o.Customer)
    .AsSplitQuery()
    .ToListAsync();
```

## 5. Use compiled queries for hot paths

A query executed thousands of times pays the translation cost every time.
Compile it once:

```csharp
private static readonly Func<AppDbContext, int, Task<Product?>> GetProduct =
    EF.CompileAsyncQuery(
        (AppDbContext db, int id) =>
            db.Products.FirstOrDefault(p => p.Id == id));
```

## 6. Raw SQL when LINQ cannot express it

For reporting queries or bulk updates, raw SQL is honest and fast:

```csharp
var count = await _db.Database
    .SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM Products")
    .SingleAsync();

await _db.Database.ExecuteSqlRawAsync(
    "UPDATE Products SET Price = Price * 1.1 WHERE CategoryId = {0}", categoryId);
```

Always use parameters (`{0}` or `SqlParameter`), never string-concatenated
values — that is SQL injection.

## Checklist

- `AsNoTracking()` on every read-only query.
- `Select()` projections instead of whole entities.
- `Include()` once instead of lazy loading in a loop.
- An index on every column used in `Where`, `OrderBy` or `Join`
  (see `notes/sql-server-indexes-basics.md`).
- `ToListAsync()` once at the end — never call it inside a loop.
- Measure with `ToQueryString()` or logging before guessing: enable
  `options.LogTo(Console.WriteLine)` in development and look at the SQL
  EF Core actually sends.
