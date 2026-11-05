# ASP.NET Core Routing

Routing maps incoming HTTP requests to endpoints (minimal API handlers or controller actions) based on the URL path, HTTP method, and optional constraints.

## Minimal API routes

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello, World!");
app.MapGet("/products/{id:int}", (int id) => $"Product {id}");
app.MapPost("/products", (Product product) => Results.Created($"/products/{product.Id}", product));

app.Run();

public record Product(int Id, string Name, decimal Price);
```

## Route templates and parameters

- `{id}` — a required route value, bound to the handler parameter by name.
- `{id:int}` — a route constraint; the segment must parse as an `int`, otherwise the route does not match (404).
- `{name?}` — an optional parameter.
- `{*slug}` — a catch-all that matches the rest of the path.
- Common constraints: `int`, `long`, `guid`, `bool`, `alpha`, `minlength(3)`, `range(1,100)`.

## Attribute routing (controllers)

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(new { Id = id });

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string name) => Ok(name);
}
```

`[controller]` is replaced by the controller name without the "Controller" suffix, so the route becomes `api/products`.

## Key points

- Routes are matched in registration order; put more specific routes first.
- `MapGet`/`MapPost`/`MapPut`/`MapDelete` match one HTTP method; `Map` matches any method.
- Query strings (`?name=...`) are not part of route matching — bind them with `[FromQuery]` or a matching parameter name.
- Use `Results` helpers (`Results.Ok`, `Results.NotFound`, `Results.Created`) to return typed responses from minimal APIs.
- Route values can be read in a handler via `HttpContext.Request.RouteValues`.

*Learning log — 5 Nov 2026*
