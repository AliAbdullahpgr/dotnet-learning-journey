# ASP.NET Core Minimal APIs

## What are they?

Minimal APIs (introduced in .NET 6) let you build HTTP APIs with
almost no ceremony — just `Program.cs`, no controllers, no startup
class. They are ideal for small services, microservices, and
prototypes.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/hello", () => "Hello World");

app.Run();
```

## Route parameters and query strings

Values come straight from the route or the query string:

```csharp
app.MapGet("/users/{id}", (int id) => $"User {id}");
app.MapGet("/search", (string? q) => $"Searching for {q}");
```

## Reading a JSON body

A complex parameter is automatically bound from the JSON request
body, and a simple in-memory store makes the idea concrete:

```csharp
var todos = new List<Todo>();

app.MapPost("/todos", (Todo todo) =>
{
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapGet("/todos", () => todos);

record Todo(int Id, string Title, bool Done);
```

## Results helpers

Return `Results.Ok(...)`, `Results.NotFound()`,
`Results.BadRequest(...)` or `Results.Created(...)` instead of raw
values when you need a specific status code.

## Route groups

Group related endpoints under a common prefix:

```csharp
var api = app.MapGroup("/api/todos");
api.MapGet("/", () => todos);
api.MapPost("/", (Todo todo) => Results.Created($"/api/todos/{todo.Id}", todo));
```

## Dependency injection

Services registered in the container can be injected straight into
the handler's parameters:

```csharp
builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();
app.MapGet("/todos", (ITodoStore store) => store.GetAll());
```

## When to use them — and when not to

- Use minimal APIs for small services, webhooks, and prototypes.
- Prefer controllers for large applications: they organise better
  with filters, model binding, versioning, and many endpoints.
- Add OpenAPI/Swagger with `AddEndpointsApiExplorer()` and
  `AddSwaggerGen()` (or the built-in OpenAPI support in .NET 9+)
  so the API stays documented and testable.
