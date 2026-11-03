# ASP.NET Core Model Validation

Model validation checks incoming data before your controller action runs.
Annotate the model with data annotations, and ASP.NET Core validates the
request body automatically when the action parameter is decorated with
`[ApiController]`.

```csharp
public class CreateTodoDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Priority { get; set; }

    [EmailAddress]
    public string? NotifyEmail { get; set; }
}

[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateTodoDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState); // 400 with field errors
        // ... save and return 201
        return CreatedAtAction(nameof(Create), dto);
    }
}
```

## Key points

- `[ApiController]` already returns a 400 response for invalid models, so
  the explicit `ModelState.IsValid` check is optional but makes the flow
  clear and keeps the action testable.
- Validation runs on the bound model only — it never replaces
  authorization or business-rule checks (e.g. "does this user own the
  todo?").
- For rules that span several fields (e.g. "end date after start date"),
  implement `IValidatableObject` or a custom `ValidationAttribute`.
- Client-side validation (e.g. in a form) is a convenience, not a
  substitute: always validate on the server, because any request can be
  sent by hand.
