# Clean Architecture (C#)

Clean Architecture organizes an app into concentric layers so the business
rules never depend on frameworks, databases or the web. The dependency rule
is the whole idea: **dependencies point inward**. The outer layers know
about the inner ones — never the other way round.

## The layers

- **Domain** (innermost) — entities, value objects and domain rules. No
  references to EF Core, ASP.NET Core or any infrastructure. A `Product`
  that refuses a negative price lives here.
- **Application** — use cases: commands, queries and their handlers
  (often with MediatR). It defines the *ports* it needs as interfaces,
  e.g. `IProductRepository`, but never implements them.
- **Infrastructure** — the implementations: EF Core repositories, the
  `DbContext`, email senders, file storage, external APIs.
- **Presentation / API** — controllers, minimal API endpoints, DTOs and
  request/response mapping. This layer only translates HTTP into
  application calls.

## Why dependencies point inward

If the domain depended on EF Core, changing the database would mean
rewriting business rules. With the rule the other way round, you can swap
SQL Server for SQLite, or swap a real email sender for a fake one in
tests, without touching the domain or the use cases.

```csharp
// Application layer — a port, not an implementation.
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
}

// Infrastructure layer — the adapter that fulfils the port.
public class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public EfProductRepository(AppDbContext db) => _db = db;

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
    }
}
```

## A typical project layout

```
src/
  MyApp.Domain/          # entities, value objects, domain rules
  MyApp.Application/     # use cases, ports (interfaces), DTOs
  MyApp.Infrastructure/  # EF Core, repositories, external services
  MyApp.Api/             # controllers, Program.cs, DI wiring
```

## Rules that keep it clean

- The Domain project references **nothing** — no NuGet packages for
  data access or web frameworks.
- The Application project references only Domain.
- Infrastructure references Application (to implement its interfaces)
  and Domain.
- The Api project references all three, but only to wire up dependency
  injection in `Program.cs` — controllers talk to the Application layer,
  not to the `DbContext` directly.

## When to use it

For a real backend with rules that must survive — an app that will grow,
be tested and be maintained — the structure pays for itself. For a tiny
CRUD prototype it is overkill: a single project with folders is enough
until the rules get complicated.
