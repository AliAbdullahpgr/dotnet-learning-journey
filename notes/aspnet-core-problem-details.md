# Problem Details (RFC 7807) in ASP.NET Core

`ProblemDetails` is the standard shape for API errors (RFC 7807):
a JSON object with `type`, `title`, `status`, `detail` and `instance`,
plus any extra fields you add. Clients can parse errors the same way
for every endpoint instead of guessing at ad-hoc shapes.

## What a problem response looks like

```json
{
  "type": "https://example.com/errors/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "No product exists with id 42.",
  "instance": "/api/products/42"
}
```

## Returning one from an endpoint

Minimal APIs and controllers both have a `Results.Problem` /
`Problem()` helper:

```csharp
app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null
        ? Results.Problem(
            title: "Resource not found",
            detail: $"No product exists with id {id}.",
            statusCode: StatusCodes.Status404NotFound,
            type: "https://example.com/errors/not-found")
        : Results.Ok(product);
});
```

## Validation errors come free

With `[ApiController]`, a model-validation failure already returns a
`ValidationProblemDetails` (HTTP 400) with an `errors` dictionary of
field → messages. You do not need to build that shape yourself.

## Unhandled exceptions as Problem Details

`UseExceptionHandler` can turn any unhandled exception into a problem
response instead of an empty 500:

```csharp
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = app.Environment.IsDevelopment() ? exception?.Message : null,
            Instance = context.Request.Path
        };
        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem);
    });
});
```

Never leak stack traces or internal messages in production — return a
generic detail and log the real exception server-side.

## Customizing the shape

- Add an extension property (for example `traceId`) by building the
  `ProblemDetails` object yourself, or by customizing
  `IProblemDetailsService` / `ProblemDetailsOptions` in .NET 7+.
- Use a stable `type` URI per error kind so clients can switch on it.
  The URI does not have to resolve to a real page, but it should be
  unique to your API.

## Takeaway

Use `ProblemDetails` for every error your API returns: `Results.Problem`
for expected failures, the built-in validation problem for bad input,
and an exception handler for the rest. One error shape makes the API
far easier to consume — and to debug.
