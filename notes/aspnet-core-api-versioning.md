# API Versioning in ASP.NET Core

APIs change, but clients do not all upgrade at the same time. API
versioning lets v1 and v2 of an endpoint live side by side, so old clients
keep working while new clients get the improved shape.

## URL versioning (the most common style)

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // adds api-supported-versions header
});
```

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "v1 product list" });
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "v2 product list with prices" });
}
```

`GET /api/v1/products` and `GET /api/v2/products` now hit different
controllers.

## Other versioning styles

- **Query string** — `GET /api/products?api-version=2.0`. Easy to test in
  a browser, but the version is not part of the route.
- **Header** — `api-version: 2.0` in the request headers. Keeps URLs
  clean, but the version is invisible in the URL.
- **Media type** — `Accept: application/json; v=2.0`. The purest REST
  style, and the hardest for clients to use.

URL versioning is the easiest to understand and debug, which is why most
teams pick it.

## Marking versions as deprecated

```csharp
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
```

Clients calling v1 get an `api-deprecated-versions: 1.0` response header —
a polite warning before the version is removed.

## Rules to remember

- Version the API, not every endpoint: bump a version only when a
  breaking change ships (a removed field, a changed type, a new required
  parameter).
- Adding a field is not breaking — old clients ignore unknown fields.
- Removing a version is a breaking change too: announce it, deprecate
  first, and give clients a deadline.
- Keep the version in one place: the route template
  `api/v{version:apiVersion}/...` does that for every controller.
- Document each version separately in Swagger so clients can see exactly
  what changed.
