# Common Table Expressions (CTEs) in SQL

A common table expression (CTE) is a named, temporary result set defined with `WITH` at the start of a query, so a complex query can be written as readable steps instead of one long nested statement.

## Key points

- Define a CTE as `WITH Name AS (SELECT ...) SELECT ... FROM Name;` and use it in the statement that immediately follows.
- A CTE exists only for that one statement; it is not stored like a view or a table.
- Define several CTEs in one `WITH` clause, separated by commas, and let later CTEs build on earlier ones.
- CTEs make a query easier to read and debug, because each step has a name and can be tested on its own.
- A recursive CTE refers to itself with `UNION ALL`: an anchor query gives the first rows and a recursive query adds the next level, which is useful for hierarchies such as employees and managers.
- In SQL Server, a recursive CTE needs a termination condition, or it will keep producing rows until the default recursion limit of 100 is reached.
- A CTE can also feed an `INSERT`, `UPDATE`, or `DELETE`, not only a `SELECT`.
- CTEs are usually a readability tool; if the same result is needed many times in one query, check whether the database materialises it or re-runs it each time.

## Example

```sql
WITH HighEarners AS (
    SELECT EmployeeName, Department, Salary
    FROM Employees
    WHERE Salary > 80000
)
SELECT Department, COUNT(*) AS HighEarnerCount
FROM HighEarners
GROUP BY Department;
```
