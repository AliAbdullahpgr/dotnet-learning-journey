# Eager and Lazy Loading in EF Core

Loading strategies decide when EF Core fetches related data. The right choice avoids both missing data and a flood of extra queries.

## Key points

- Eager loading fetches related data in the same query, with `Include` and `ThenInclude`; it is the default recommendation when you know you need the related data.
- Lazy loading fetches a navigation property only when it is first accessed. It needs lazy-loading proxies (`UseLazyLoadingProxies` from Microsoft.EntityFrameworkCore.Proxies) and `virtual` navigation properties.
- Explicit loading fetches related data on demand for an entity you already have, with `context.Entry(order).Collection(o => o.Items).LoadAsync()` or `.Reference(...).LoadAsync()`.
- The N+1 problem happens when lazy loading runs one extra query per parent row; eager loading that data in one query avoids it.
- Lazy loading fails after the `DbContext` is disposed, because there is no database connection left to load from.
- Eager loading can fetch more than you need, so use `Select` to project only the fields a screen or API response actually uses.
- As a rule of thumb: eager load data you will always use, lazy load data you rarely use, and explicitly load data you need only sometimes.

## Example

```csharp
// Eager loading: orders arrive with their customers, in one query.
List<Order> orders = await context.Orders
    .Include(o => o.Customer)
    .ThenInclude(c => c.Address)
    .ToListAsync();

// Explicit loading: load the items of one order you already have.
await context.Entry(orders[0])
    .Collection(o => o.Items)
    .LoadAsync();
```
