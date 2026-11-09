# CORS in ASP.NET Core

CORS (Cross-Origin Resource Sharing) lets a browser page hosted on one origin safely call an API on another origin, by letting the API say which origins are allowed.

## Key points

- Browsers enforce the same-origin policy: a page at `https://app.example.com` cannot read a response from `https://api.example.com` unless the API allows it with CORS headers.
- CORS is a browser rule, not API security: tools such as curl ignore it, so it never replaces authentication or authorisation.
- Register a policy with `builder.Services.AddCors`, for example `AddPolicy("AllowFrontend", ...)`, and apply it with `app.UseCors("AllowFrontend")`.
- `WithOrigins("https://app.example.com")` allows only that origin; `AllowAnyOrigin()` allows all of them and is too open for an API that handles real user data.
- Control the rest with `AllowAnyHeader()`, `AllowAnyMethod()`, `WithHeaders(...)`, and `WithMethods(...)`.
- `AllowCredentials()` is needed for cookies or authorisation headers, but it cannot be combined with `AllowAnyOrigin()`; list the exact origins instead.
- The browser first sends a preflight `OPTIONS` request for non-simple requests; ASP.NET Core answers it automatically when CORS is configured.
- Put `UseCors` early in the pipeline, before `UseAuthentication` and `UseAuthorization`, so the CORS headers are added to every response, including errors.

## Example

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://app.example.com")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowFrontend");
```
