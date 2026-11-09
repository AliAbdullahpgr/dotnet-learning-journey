# Filters in ASP.NET Core

Filters run code before or after specific stages of request processing. They are the place for cross-cutting concerns such as authentication, logging, caching, exception handling, and validation, so controllers stay focused on their own work.

## Key points

- Filter types run in this order: authorization filters, resource filters, action filters, result filters; exception filters run when an unhandled exception occurs.
- Action filters run before and after an action executes. Setting `context.Result` in `OnActionExecuting` short-circuits the pipeline — the action never runs and the result is returned directly.
- Built-in filters include `[Authorize]`, `[ResponseCache]`, and `[ServiceFilter]`; custom filters implement interfaces such as `IActionFilter`, `IAsyncActionFilter`, `IResultFilter`, or `IExceptionFilter`.
- Filters can be applied globally, per controller, or per action. Global filters run for every request; attribute filters run only where they are applied.

## Example

```csharp
public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"Executing {context.ActionDescriptor.DisplayName}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"Executed {context.ActionDescriptor.DisplayName}");
    }
}
```

Register it globally with `builder.Services.AddControllers(options => options.Filters.Add<LogActionFilter>());`, or apply it to one action with `[ServiceFilter(typeof(LogActionFilter))]`.
