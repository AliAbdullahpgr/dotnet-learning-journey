# Change Tracking in EF Core

When EF Core loads entities from the database, the `DbContext` keeps track of them. This change tracker records each entity's state, so `SaveChanges` knows which SQL statements to generate.

## Key points

- Every tracked entity has a state: `Added`, `Modified`, `Deleted`, `Unchanged`, or `Detached`.
- `SaveChanges` inserts entities in the `Added` state, updates `Modified` ones, and deletes `Deleted` ones; `Unchanged` entities are left alone.
- EF Core detects changes by comparing each entity's current values with a snapshot taken when the entity was first tracked.
- `AsNoTracking()` queries skip tracking entirely: they are faster for read-only scenarios, but the returned entities cannot be saved back unless they are attached first.
- The default tracking behaviour can be changed with `QueryTrackingBehavior.NoTracking`, and a single entity can be detached with `context.Entry(entity).State = EntityState.Detached`.

## Example

```csharp
var product = await context.Products.FindAsync(1);
product.Price = 19.99m; // state becomes Modified
await context.SaveChangesAsync(); // issues an UPDATE for the changed row
```

Setting `Price` marks the entity as `Modified`, and `SaveChangesAsync` sends only the update for that row to the database.
