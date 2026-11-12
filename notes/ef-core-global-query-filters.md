# EF Core Global Query Filters

A global query filter is a `Where` clause EF Core applies to every
query on an entity, automatically. It is configured once in
`OnModelCreating` and you never have to remember it again.

## The two classic uses

- **Soft delete** — rows are flagged `IsDeleted` instead of removed,
  and normal queries never see them.
- **Multi-tenancy** — every query is automatically scoped to the
  current tenant.

```csharp
public class AppDbContext : DbContext
{
    private readonly int _currentTenantId;

    public AppDbContext(DbContextOptions<AppDbContext> options, int currentTenantId)
        : base(options) => _currentTenantId = currentTenantId;

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Soft delete: deleted products are invisible to normal queries.
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        // Multi-tenancy: only this tenant's orders are ever returned.
        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => o.TenantId == _currentTenantId);
    }
}
```

The filter value (`_currentTenantId`) is captured from the DbContext
instance, so each request gets its own scope as long as the context is
scoped per request — which is the default in ASP.NET Core.

## Opting out

```csharp
// Admin screens and cleanup jobs need the hidden rows too:
var allProducts = context.Products.IgnoreQueryFilters().ToList();
```

`IgnoreQueryFilters()` removes *all* filters on that query, so use it
only where you genuinely need the unfiltered set.

## Rules and pitfalls

- Filters apply to `Include` and navigation loading as well — a
  filtered-out related row simply never loads.
- A filter cannot reference a navigation property of the filtered
  entity; keep the condition on the entity's own columns (and values
  captured from the context).
- Filters do **not** apply to raw SQL (`FromSqlRaw`) or to direct
  `ExecuteDelete` / `ExecuteUpdate` calls — those bypass them, which
  is a real data-leak risk for soft delete if you mix approaches.
- Combining soft delete with unique indexes needs care: a deleted row
  still occupies its unique value. A common fix is a filtered unique
  index (`WHERE IsDeleted = 0`) in the migration.
- Test the filter: write one test that deletes a row and asserts a
  normal query no longer returns it — a missing filter fails silently.

## Takeaway

Global query filters turn soft delete and multi-tenancy from a rule
every developer must remember into a rule the framework enforces.
Define them once, bypass them explicitly with `IgnoreQueryFilters()`
only where needed, and remember they do not cover raw SQL.
