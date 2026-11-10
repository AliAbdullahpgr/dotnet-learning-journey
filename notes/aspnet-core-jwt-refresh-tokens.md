# JWT Refresh Tokens in ASP.NET Core

An access token should be short-lived (about 15 minutes), so a stolen
token is only useful for a short time. A refresh token is the second,
long-lived token that lets the client get a new access token without
asking the user to log in again.

## The two-token flow

1. The user logs in. The API returns an **access token** (15 minutes) and
   a **refresh token** (7–30 days).
2. The client stores the access token in memory and the refresh token
   securely (HttpOnly cookie or secure storage on mobile).
3. When the access token expires, the client calls `/api/auth/refresh`
   with the refresh token.
4. The API checks the refresh token, and if it is valid and not revoked,
   returns a fresh pair of tokens.
5. The user never sees a login screen until the refresh token itself
   expires or is revoked.

## Storing refresh tokens

Unlike access tokens, refresh tokens must be stored server-side so they
can be revoked — for example when the user logs out or changes their
password. A simple table is enough:

```csharp
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = ""; // random, hashed before storage
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
```

Generate the token value from a cryptographic random source, never from
a GUID or a timestamp:

```csharp
public static string GenerateRefreshToken() =>
    Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
```

## The refresh endpoint

```csharp
[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
{
    var stored = await _db.RefreshTokens
        .FirstOrDefaultAsync(t => t.Token == Hash(request.RefreshToken));

    if (stored is null || !stored.IsActive)
    {
        return Unauthorized("Invalid refresh token.");
    }

    // Rotation: the old token is revoked and a new pair is issued.
    stored.RevokedAt = DateTime.UtcNow;
    var newRefresh = new RefreshToken
    {
        UserId = stored.UserId,
        Token = Hash(RefreshTokenService.GenerateRefreshToken()),
        ExpiresAt = DateTime.UtcNow.AddDays(7),
    };
    _db.RefreshTokens.Add(newRefresh);
    await _db.SaveChangesAsync();

    var newAccess = _tokens.CreateToken(stored.UserId);
    return Ok(new { AccessToken = newAccess, RefreshToken = newRefresh.Token });
}
```

## Rules to remember

- **Rotate on every use** — each refresh returns a new refresh token and
  revokes the old one. If an old token is presented twice, it means the
  token was stolen: revoke the whole token family and force a re-login.
- **Hash refresh tokens in the database**, like passwords. A leaked table
  must not hand out working tokens.
- **Revoke on logout and password change** — set `RevokedAt` for all of
  the user's tokens.
- **Never put a refresh token in a URL or in localStorage** on the web;
  an HttpOnly, Secure, SameSite cookie is the safer home.
- Access tokens stay stateless and short-lived; refresh tokens are the
  only part that needs a database lookup, and that lookup happens only
  once per access-token lifetime.
