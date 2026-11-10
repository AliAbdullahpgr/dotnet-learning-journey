# Authorization in ASP.NET Core — Deep Dive

Authentication answers "who are you?" Authorization answers "are you
allowed to do this?" Once a user is authenticated (by cookie or JWT),
authorization decides which endpoints, pages and data they may touch.

## Role-based authorization

The simplest form: check the user's role claim.

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("api/products/{id}")]
public IActionResult Delete(int id) => NoContent();

[Authorize(Roles = "Admin,Manager")]
[HttpPost("api/products")]
public IActionResult Create(ProductDto dto) => Ok();
```

Roles come from the `ClaimTypes.Role` claim in the token or cookie. They
are quick to write but go stale fast: every new rule means a new role,
and roles say nothing about *which* resource is being touched.

## Claims-based authorization

Claims describe facts about the user: department, clearance level, or a
permission like `products:edit`. Check a single claim with a policy:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditProducts", policy =>
        policy.RequireClaim("permission", "products:edit"));
});
```

```csharp
[Authorize(Policy = "CanEditProducts")]
[HttpPut("api/products/{id}")]
public IActionResult Update(int id, ProductDto dto) => Ok();
```

## Policy with a custom requirement

When the rule needs logic — for example "only the author may edit this
post" — write a requirement and a handler:

```csharp
public class IsAuthorRequirement : IAuthorizationRequirement { }

public class IsAuthorHandler : AuthorizationHandler<IsAuthorRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsAuthorRequirement requirement,
        Post post)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == post.AuthorId.ToString())
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
```

Register the handler and the policy, then authorize against the resource:

```csharp
builder.Services.AddSingleton<IAuthorizationHandler, IsAuthorHandler>();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("IsAuthor", policy =>
        policy.Requirements.Add(new IsAuthorRequirement())));
```

```csharp
var post = await _db.Posts.FindAsync(id);
var result = await _authorizationService.AuthorizeAsync(User, post, "IsAuthor");
if (!result.Succeeded) return Forbid();
```

This is resource-based authorization: the decision depends on the data,
not just on who the user is.

## Rules to remember

- `[Authorize]` without a role or policy only checks that the user is
  logged in — it grants nothing by itself.
- Prefer policies over roles in attributes: a policy has a name that
  describes the rule, and its logic lives in one place.
- Authorization runs after authentication, so `UseAuthentication()` must
  come before `UseAuthorization()` in `Program.cs`.
- Never trust a role or claim the client sends you: claims must come from
  your own token issuer, validated against your signing key.
- Test authorization with a user who lacks the permission, not only with
  an admin — the "forbidden" path is the one that breaks in production.
