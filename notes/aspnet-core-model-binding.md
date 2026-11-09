# Model Binding in ASP.NET Core

Model binding maps data from an HTTP request onto the parameters of a controller action or Razor Page handler, so actions work with typed values instead of raw request data.

## Where the data comes from

- **Route values** — segments of the URL, e.g. `/products/5` binds `id = 5`.
- **Query string** — `?page=2&size=10` binds `page` and `size`.
- **Form fields** — posted form data binds to a complex type's properties.
- **Request body** — JSON (or another format) binds to a complex type with `[FromBody]`.
- **Headers** — single values bind with `[FromHeader]`.

## Key points

- Simple types (int, string, bool, Guid) are bound from route, query string, or form data by default; complex types are bound from the request body (or form data) by default.
- A name must match for the binding to work: the binder matches request keys to parameter or property names (case-insensitive).
- Only one parameter per action may be bound from the body.
- If binding fails (for example, "abc" for an int), the parameter gets its default value and `ModelState` records an error — always check `ModelState.IsValid` before trusting the input.
- Validation attributes such as `[Required]` and `[Range(1, 100)]` run after binding and add errors to `ModelState`.

## Example

```csharp
[HttpGet("search")]
public IActionResult Search([FromQuery] string text, [FromQuery] int page = 1)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var results = _catalog.Search(text, page);
    return Ok(results);
}
```

`GET /search?text=keyboard&page=2` binds `text = "keyboard"` and `page = 2`; a missing `page` falls back to the default value `1`.
