# GROUP BY and HAVING in SQL

`GROUP BY` groups rows that share the same values into summary rows, so aggregate functions run once per group instead of once per table.

## Key points

- Aggregate functions such as `COUNT`, `SUM`, `AVG`, `MIN`, and `MAX` compute one value per group.
- Every column in the `SELECT` list must either appear in the `GROUP BY` clause or be wrapped in an aggregate function.
- `WHERE` filters individual rows **before** grouping; `HAVING` filters groups **after** grouping.
- `HAVING` can use aggregate conditions (for example `HAVING COUNT(*) > 5`); `WHERE` cannot, because aggregates are not computed yet at that stage.
- Logical order of evaluation: `FROM` -> `WHERE` -> `GROUP BY` -> `HAVING` -> `SELECT` -> `ORDER BY`.

## Example

```sql
SELECT Category, COUNT(*) AS ProductCount, AVG(Price) AS AvgPrice
FROM Products
WHERE Price > 0
GROUP BY Category
HAVING COUNT(*) >= 3
ORDER BY AvgPrice DESC;
```

This returns only the categories that have at least three products, together with each category's product count and average price.
