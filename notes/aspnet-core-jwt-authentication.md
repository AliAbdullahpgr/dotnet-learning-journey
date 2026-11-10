# JWT Authentication in ASP.NET Core

A JSON Web Token (JWT) is a compact, signed token that an API issues after
a user logs in. The client sends it back in the `Authorization` header on
every request, so the API can identify the user without keeping server-side
session state. This is the standard way to secure ASP.NET Core Web APIs.

## What a JWT contains

A token has three Base64 parts, separated by dots:

- **Header** — the signing algorithm, e.g. `HS256`.
- **Payload** — claims: the user id (`sub`), name, roles, expiry (`exp`).
- **Signature** — created from header + payload + a secret key. If anyone
  changes the payload, the signature no longer matches and the token is
  rejected.

Never put secrets in a token: the payload is only encoded, not encrypted,
so anyone can read it.

## Issuing a token at login

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public string CreateToken(string userId, string userName, string secretKey)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, userName),
        new Claim(ClaimTypes.Role, "User"),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "my-api",
        audience: "my-app",
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

## Validating tokens on the API

Register JWT bearer authentication in `Program.cs`:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "my-api",
            ValidAudience = "my-app",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```

Then protect endpoints with `[Authorize]`:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get() =>
        Ok(new { User = User.Identity?.Name });
}
```

The client calls the API with:

```http
Authorization: Bearer <token>
```

## Rules to remember

- Keep the secret key in configuration or a secret store, never in code.
- Keep access tokens short-lived (minutes, not days). Long-lived access is
  what refresh tokens are for.
- Always use HTTPS: a token sent over plain HTTP can be stolen and reused.
- `UseAuthentication()` must come before `UseAuthorization()` in the
  pipeline, or authorization will run before the user is identified.
