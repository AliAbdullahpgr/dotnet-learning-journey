# Repository Pattern in C#

The Repository pattern hides data-access details behind an interface, so
the rest of the application works with plain objects and never sees SQL,
EF Core or any other storage technology.

## The pattern

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task SaveChangesAsync();
}
```

One implementation talks to EF Core:

```csharp
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<Product?> GetByIdAsync(int id) =>
        await _db.Products.FindAsync(id);

    public async Task<List<Product>> GetAllAsync() =>
        await _db.Products.AsNoTracking().ToListAsync();

    public async Task AddAsync(Product product) =>
        await _db.Products.AddAsync(product);

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}
```

Register it in `Program.cs` and inject the interface into controllers or
services:

```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

## What you gain

- **Testability** — a service can be tested with a fake repository that
  returns fixed data, with no database at all.
- **Swappable storage** — moving from EF Core to Dapper (or to an API)
  means writing one new class, not editing every service.
- **One place for queries** — repeated queries live in the repository
  instead of being copied across controllers.

## When NOT to use it

- EF Core's `DbContext` is already a repository plus a unit of work. For
  a small API with simple CRUD, injecting the `DbContext` directly is
  fine, and a repository layer just adds files.
- Do not build a generic repository (`IRepository<T>` with Get/Add/Delete
  for every entity) that hides EF Core's real power — `IQueryable`,
  includes and projections. A generic wrapper usually makes queries worse,
  not better.
- Keep repositories narrow: one repository per aggregate (Product,
  Order), with methods named after real use cases, not a copy of every
  `DbSet` method.
