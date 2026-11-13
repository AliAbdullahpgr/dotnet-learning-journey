# Endpoint Filters in ASP.NET Core (ASP.NET Core)

Endpoint filters run around a single minimal API endpoint. They are the minimal-API answer to the question "how do I add validation or logging to just this endpoint?" — lighter than middleware, which runs for every request, and more focused than a full MVC action filter.

## What an Endpoint Filter Is

An endpoint filter implements `IEndpointFilter` and receives an `EndpointFilterInvocationContext` plus a delegate for the next step in the pipeline. It can inspect or change the arguments, short-circuit the request by returning a result early, or run code after the handler finishes.

```csharp
public class LoggingFilter : IEndpointFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger) => _logger = logger;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        _logger.LogInformation("Calling {Path}", context.HttpContext.Request.Path);
        var result = await next(context);
        _logger.LogInformation("Finished with {Result}", result?.GetType().Name);
        return result;
    }
}
```

## A Validation Filter

A common use is validating the request before the handler runs. The filter reads the argument, and if it is invalid it returns a `Results.ValidationProblem` instead of calling the handler at all.

```csharp
public class TodoValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var todo = context.GetArgument<Todo>(0);

        if (string.IsNullOrWhiteSpace(todo.Title))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["Title"] = ["Title is required."]
            });

        return await next(context);
    }
}

public record Todo(string Title, bool Done);
```

## Registering a Filter

Filters are attached per endpoint with `AddEndpointFilter`, or to a whole route group with `AddEndpointFilter` on the group. A filter registered on a group applies to every endpoint in that group.

```csharp
var todos = app.MapGroup("/todos")
    .AddEndpointFilter<LoggingFilter>();

todos.MapPost("/", (Todo todo) => Results.Created($"/todos/1", todo))
    .AddEndpointFilter<TodoValidationFilter>();
```

Filters can also be registered as a lambda for a one-off case, without a separate class:

```csharp
app.MapGet("/hello", () => "Hello")
    .AddEndpointFilter(async (context, next) =>
    {
        if (context.HttpContext.Request.Headers.ContainsKey("X-Blocked"))
            return Results.Forbid();
        return await next(context);
    });
```

## Ordering

Filters run in the order they are added: the first filter added is the outermost, so it runs first on the way in and last on the way out. Group-level filters run before endpoint-level filters, because the group filter wraps the whole group.

## Filters vs Middleware

Middleware runs for every request that reaches it, regardless of the endpoint — authentication, CORS, and exception handling belong there. Endpoint filters run only for the endpoints they are attached to, and they can see the endpoint's typed arguments and its return value. Use middleware for cross-cutting concerns that apply everywhere, and endpoint filters for logic that belongs to one endpoint or one group, such as input validation, per-endpoint logging, or a feature flag check.
