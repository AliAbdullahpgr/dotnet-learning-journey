# ASP.NET Core Middleware Pipeline

In ASP.NET Core, a request passes through a pipeline of middleware components. Each component can inspect the request, pass it on, or stop it and return a response.

## Order matters

Middleware runs in the order it is registered, and the response travels back in reverse order. Authentication must therefore run before authorization, and exception handling is usually registered first so it can catch errors from later components.

## Example: a simple logging middleware

```csharp
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next(context); // pass the request to the next component
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.Run(async context =>
{
    await context.Response.WriteAsync("Hello from the end of the pipeline!");
});
```

## Key ideas

- `app.Use` adds a component that calls `next` to continue the pipeline.
- `app.Run` adds a terminal component: it does not call `next`, so nothing after it runs.
- A component that returns a response without calling `next` is called short-circuiting. Static files and authentication challenges both use this.
