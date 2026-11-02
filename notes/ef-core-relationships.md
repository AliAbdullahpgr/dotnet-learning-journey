# EF Core Relationships

EF Core maps relationships between entities using navigation properties and foreign keys.

## One-to-many

One blog has many posts, and each post belongs to one blog:

```csharp
public class Blog
{
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int BlogId { get; set; } // foreign key
    public Blog Blog { get; set; } = null!;
}
```

By convention, EF Core sees `BlogId` plus the `Blog` navigation and builds the one-to-many relationship without extra configuration.

## Explicit configuration with the Fluent API

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>()
        .HasMany(b => b.Posts)
        .WithOne(p => p.Blog)
        .HasForeignKey(p => p.BlogId);
}
```

## Other shapes

- One-to-one: each entity has at most one related entity, for example a user and a profile.
- Many-to-many: EF Core 5 and later creates the join table automatically from two collection navigations, for example posts and tags.
