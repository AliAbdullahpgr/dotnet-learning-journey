# SQL Server Joins

A join combines rows from two tables using a related column, usually a foreign key.

```sql
-- Sample tables
-- Customers(CustomerId, Name)
-- Orders(OrderId, CustomerId, Total)
```

## INNER JOIN — only matching rows

```sql
SELECT c.Name, o.OrderId, o.Total
FROM Customers c
INNER JOIN Orders o ON o.CustomerId = c.CustomerId;
```

Customers with no orders, and orders with no customer, are left out.

## LEFT JOIN — all rows from the left table

```sql
SELECT c.Name, o.OrderId
FROM Customers c
LEFT JOIN Orders o ON o.CustomerId = c.CustomerId;
```

Customers with no orders still appear, with NULL in the order columns. This is the usual way to find customers who never ordered: add `WHERE o.OrderId IS NULL`.

## RIGHT JOIN and FULL OUTER JOIN

- `RIGHT JOIN` keeps all rows from the right table, with NULLs where the left side has no match.
- `FULL OUTER JOIN` keeps all rows from both tables, with NULLs on either side where there is no match.

## CROSS JOIN — every combination

```sql
SELECT c.Name, p.ProductName
FROM Customers c
CROSS JOIN Products p;
```

Useful for combinations (e.g. sizes x colours); dangerous on large tables because the row count multiplies.

## Key points

- Always join ON the key columns; joining without a condition is a cross join.
- Filter in the ON clause to affect matching, and in WHERE to filter the final result — for LEFT JOINs this distinction changes the output.
- Prefer explicit JOIN syntax over comma-separated tables in FROM; it is clearer and harder to get wrong.
