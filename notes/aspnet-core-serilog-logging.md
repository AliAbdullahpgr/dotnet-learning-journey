# Serilog Structured Logging in ASP.NET Core

The built-in logger writes plain text. Serilog writes structured events:
each log entry carries named properties, so you can search logs by
`UserId` or `OrderId` instead of grepping text. It is the standard logging
choice for .NET apps.

## Set up Serilog

Add the `Serilog.AspNetCore` package, then configure it in `Program.cs`:

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

Or configure from appsettings.json — better, because log levels can change
without rebuilding:

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Log with properties, not string interpolation

```csharp
// Bad: the user id is trapped inside a string.
_logger.LogInformation($"Order {orderId} placed by user {userId}.");

// Good: UserId and OrderId are searchable properties.
_logger.LogInformation(
    "Order {OrderId} placed by user {UserId}.", orderId, userId);
```

Serilog also captures context automatically — enrich the logger once:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Application", "MyApi")
    .WriteTo.Console()
    .CreateLogger();
```

## Sinks: where logs go

- **Console** — development and containers.
- **File** — simple, durable, good for small apps.
- **Seq** — a log server with a web UI for searching structured events.
- **Elasticsearch / Application Insights** — for production systems with
  many servers.

One logger, many sinks: add a sink and the same events flow there, no
code changes needed.

## Rules to remember

- Log levels: `Verbose` (trace), `Debug`, `Information`, `Warning`,
  `Error`, `Fatal`. Production usually runs at `Information`.
- Never log secrets: passwords, tokens, connection strings, or personal
  data. A log file is often less protected than the database.
- Log the *why*, not just the *what*: "Payment failed for order 42:
  card declined" beats "Error in PaymentService".
- Use scopes to group related logs: `using (_logger.BeginScope("Order {OrderId}", id))`
  attaches the order id to every log inside the block.
