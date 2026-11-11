# CQRS with MediatR (C#)

CQRS (Command Query Responsibility Segregation) splits reads from writes:
commands change state, queries only read it. MediatR is the small library
that wires this up in .NET — a controller sends a request, MediatR finds
the matching handler, and the handler does the work.

## Commands vs queries

- **Command** — an intent to change something: `CreateOrderCommand`,
  `UpdateProductPriceCommand`. It returns little (an id, or nothing).
- **Query** — a question: `GetOrderByIdQuery`, `ListProductsQuery`. It
  never changes state and returns data, often a purpose-built DTO.

Keeping them apart makes each side simpler: queries can skip the domain
rules and read straight from a fast projection, while commands focus on
validation and business rules.

## A command and its handler

```csharp
public record CreateProductCommand(string Name, decimal Price)
    : IRequest<int>;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly AppDbContext _db;

    public CreateProductHandler(AppDbContext db) => _db = db;

    public async Task<int> Handle(
        CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product(request.Name, request.Price);
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return product.Id;
    }
}
```

Sending it from a controller is one line:

```csharp
var id = await _mediator.Send(new CreateProductCommand("Keyboard", 49.99m));
```

## Queries with MediatR

```csharp
public record GetProductQuery(int Id) : IRequest<ProductDto?>;

public class GetProductHandler
    : IRequestHandler<GetProductQuery, ProductDto?>
{
    private readonly AppDbContext _db;

    public GetProductHandler(AppDbContext db) => _db = db;

    public async Task<ProductDto?> Handle(
        GetProductQuery request, CancellationToken ct)
        => await _db.Products
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync(ct);
}
```

## Pipeline behaviours

A behaviour runs around every request — the classic cross-cutting spot:

- **Validation** — run FluentValidation before the handler, and reject
  bad commands in one place instead of in every controller.
- **Logging / timing** — log the request type and how long it took.
- **Transactions** — open one database transaction per command, so a
  handler that touches several tables stays atomic.

## When CQRS helps — and when it does not

- Use it when reads and writes have genuinely different needs: a complex
  write model but simple, fast reads, or reads that deserve their own
  optimized shape.
- Do not use it for a simple CRUD app. One handler per operation doubles
  the number of files, and for basic CRUD that is pure overhead — a plain
  service class is the better call.
