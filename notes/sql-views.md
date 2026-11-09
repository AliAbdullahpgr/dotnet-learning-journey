# Views in SQL Server

A view is a saved `SELECT` query that is presented as a virtual table. The view itself stores no rows — every time it is queried, SQL Server runs the underlying query against the base tables.

## Key points

- **Simplify:** hide joins and calculations behind a simple `SELECT` from the view.
- **Security:** expose only selected columns or rows, without granting access to the base tables.
- **Stable interface:** application code can keep querying the view even if the underlying tables change.
- Simple views over a single table can be updatable (INSERT, UPDATE, DELETE work); views with aggregates, `DISTINCT`, or joins are read-only.
- Manage views with `CREATE VIEW`, `ALTER VIEW`, and `DROP VIEW`; after changing a base table, run `sp_refreshview` so the view's metadata stays in sync.
- Indexed views materialize the result set on disk, which can speed up heavy reads, but they have strict requirements and add write overhead.

## Example

```sql
CREATE VIEW CustomerOrderSummary AS
SELECT c.CustomerId, c.Name, COUNT(o.OrderId) AS OrderCount
FROM Customers c
LEFT JOIN Orders o ON o.CustomerId = c.CustomerId
GROUP BY c.CustomerId, c.Name;
```

After that, `SELECT * FROM CustomerOrderSummary;` returns one row per customer with their total order count, without repeating the join each time.
