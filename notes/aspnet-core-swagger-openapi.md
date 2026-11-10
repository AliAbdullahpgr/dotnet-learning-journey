# Swagger and OpenAPI in ASP.NET Core

Swagger UI is an interactive page that documents your API: every endpoint,
its parameters and its responses, with a "Try it out" button. It is
generated from an OpenAPI document, which ASP.NET Core can produce
automatically from your controllers.

## Enable Swagger

Add the `Swashbuckle.AspNetCore` package and register it in `Program.cs`:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Learning project API.",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

Open `https://localhost:5001/swagger` and the UI lists every endpoint.

## Document responses

Tell Swagger what each endpoint returns so clients and tests know what
to expect:

```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public IActionResult Get(int id)
{
    var product = _products.FirstOrDefault(p => p.Id == id);
    return product is null ? NotFound() : Ok(product);
}
```

## XML comments in the docs

Enable XML documentation in the `.csproj`, then feed it to Swagger:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

```csharp
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

Now a `/// <summary>` comment on a controller action appears in the UI:

```csharp
/// <summary>Returns a single product by id.</summary>
/// <param name="id">The product id.</param>
[HttpGet("{id}")]
public IActionResult Get(int id) { ... }
```

## Rules to remember

- Keep Swagger in development only (or behind authentication). A public
  Swagger page is a map of your whole API for attackers.
- Swagger describes the API; it does not test it. A green "Try it out"
  response does not mean the endpoint is correct.
- For APIs with versions, create one Swagger document per version so
  clients can browse v1 and v2 separately.
- In .NET 9+, the built-in `AddOpenApi` produces the same OpenAPI
  document without Swashbuckle; the concepts are identical.
