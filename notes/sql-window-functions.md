# Window Functions in SQL

A window function performs a calculation across a set of rows that are related to the current row, without collapsing those rows into one row the way `GROUP BY` does.

## Key points

- A window function uses an `OVER()` clause, for example `SUM(Salary) OVER ()`.
- `PARTITION BY` splits the rows into groups, like a `GROUP BY` per window, for example `PARTITION BY Department`.
- `ORDER BY` inside `OVER()` defines the order used by ranking functions and by running totals.
- `ROW_NUMBER()` gives each row a unique number within its partition; `RANK()` and `DENSE_RANK()` give tied rows the same rank, with `RANK()` leaving a gap after a tie and `DENSE_RANK()` not doing so.
- Aggregate functions such as `SUM()`, `AVG()`, and `COUNT()` can also be window functions, which makes running totals and moving averages easy.
- `LAG()` and `LEAD()` read a value from the previous or next row in the window, which is useful for comparing a row with its neighbour.
- A window function runs after `WHERE`, so filter on its result in an outer query or a CTE, not in the `WHERE` clause itself.

## Example

```sql
SELECT
    Department,
    EmployeeName,
    Salary,
    RANK() OVER (PARTITION BY Department ORDER BY Salary DESC) AS SalaryRank,
    SUM(Salary) OVER (PARTITION BY Department) AS DepartmentTotal
FROM Employees;
```
