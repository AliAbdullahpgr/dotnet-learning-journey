# Dapper Basics (C#)

Dapper is a micro-ORM: a thin wrapper over raw ADO.NET that maps query
results to objects and handles parameters, but writes no SQL for you. You
keep full control of the SQL, and Dapper removes the boilerplate of
reading rows by hand.

## Why Dapper

- **Fast** — close to raw ADO.NET speed; there is almost no overhead
  beyond mapping rows to objects.
- **Simple** — two extension methods cover most work: `Query<T>` for
  reads and `Execute` for writes.
- **Full SQL control** — window functions, CTEs, hints and vendor SQL
  work exactly as written, unlike an ORM that may generate something
  unexpected.

## Reading rows

```csharp
using var connection = new SqlConnection(connectionString);

IEnumerable<Product> products = await connection.QueryAsync<Product>(
    "SELECT Id, Name, Price FROM Products WHERE Price < @MaxPrice",
    new { MaxPrice = 100m });

Product? one = await connection.QueryFirstOrDefaultAsync<Product>(
    "SELECT Id, Name, Price FROM Products WHERE Id = @Id",
    new { Id = 42 });
```

Parameters are passed as an anonymous object — Dapper turns its
properties into SQL parameters, so the query is safe from SQL injection.
Never concatenate values into the SQL string.

## Writing data

```csharp
int affected = await connection.ExecuteAsync(
    "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)",
    new { Name = "Keyboard", Price = 49.99m });
```

`ExecuteAsync` returns the number of rows affected. For inserts that need
the new id, use `OUTPUT INSERTED.Id` in the SQL and read it back with
`QuerySingleAsync<int>`.

## Multi-mapping (one query, two objects)

```csharp
var orders = await connection.QueryAsync<Order, Customer, Order>(
    @"SELECT o.Id, o.Total, c.Id, c.Name
      FROM Orders o JOIN Customers c ON c.Id = o.CustomerId",
    (order, customer) => { order.Customer = customer; return order; },
    splitOn: "Id");
```

`splitOn` tells Dapper where one object's columns end and the next
begin — here the second `Id` column starts the `Customer`.

## Dapper vs EF Core

- EF Core tracks changes, generates SQL from LINQ and handles migrations
  — better when the model is complex and you want the ORM to manage it.
- Dapper is better for hot read paths, reporting queries and stored
  procedures, where hand-written SQL and raw speed matter more.
- They can live in the same app: EF Core for commands, Dapper for the
  heavy read queries.
