# SQL Query Optimization (SQL)

Most slow queries are not fixed by buying a bigger server. They are fixed by writing queries the optimizer can use an index for, and by measuring instead of guessing.

## Write sargable queries

A query is sargable when the engine can use an index seek on the predicate. Wrapping a column in a function usually prevents that.

```sql
-- Not sargable: function on the column forces a full scan
SELECT OrderId, Total FROM Orders WHERE YEAR(OrderDate) = 2026;

-- Sargable: the range can seek on an index over OrderDate
SELECT OrderId, Total FROM Orders
WHERE OrderDate >= '2026-01-01' AND OrderDate < '2027-01-01';

-- Not sargable: leading wildcard cannot seek
SELECT Name FROM Customers WHERE Name LIKE '%ali%';

-- Sargable: trailing wildcard can seek
SELECT Name FROM Customers WHERE Name LIKE 'ali%';
```

## Avoid SELECT *

Selecting every column forces the engine to read more pages and often turns an index seek into a key lookup or a full scan. List only the columns you need.

```sql
-- Reads every column, including large ones you never use
SELECT * FROM Products WHERE CategoryId = 5;

-- Reads only what the report needs
SELECT ProductId, Name, Price FROM Products WHERE CategoryId = 5;
```

## Covering indexes

A covering index contains every column a query needs, so the engine never touches the table itself (no key lookups).

```sql
-- Covers the query above: seek on CategoryId, Name and Price come from the index
CREATE NONCLUSTERED INDEX IX_Products_CategoryId
ON Products (CategoryId) INCLUDE (Name, Price);
```

Keep included columns narrow. An index that covers everything is just a second copy of the table, and every write pays for it.

## Parameter sniffing basics

SQL Server compiles a plan for the first parameter value it sees and reuses it. If that value is untypical (for example a rare status), later executions with common values get a bad plan.

```sql
-- OPTION (RECOMPILE) trades compile time for a plan fitted to this value
SELECT OrderId FROM Orders WHERE Status = @status OPTION (RECOMPILE);

-- OPTION (OPTIMIZE FOR UNKNOWN) uses average selectivity instead
SELECT OrderId FROM Orders WHERE Status = @status OPTION (OPTIMIZE FOR UNKNOWN);
```

Use these sparingly: `RECOMPILE` on a hot query wastes CPU, and `OPTIMIZE FOR UNKNOWN` is only a middle ground.

## Measure with the execution plan

Never optimize on instinct. In SQL Server Management Studio, enable "Include Actual Execution Plan" (Ctrl+M) and look for:

- **Table Scan / Clustered Index Scan** on a large table — a missing or unusable index.
- **Key Lookup** repeated thousands of times — the query needs a covering index.
- **Sort** with a warning — the sort spilled to disk; check the ORDER BY columns.
- **Actual vs estimated rows** far apart — stale statistics; run `UPDATE STATISTICS`.

```sql
-- Check how expensive a query really is before and after tuning
SET STATISTICS IO, TIME ON;
SELECT ProductId, Name, Price FROM Products WHERE CategoryId = 5;
SET STATISTICS IO, TIME OFF;
```

Logical reads in the `STATISTICS IO` output are the number to compare: fewer reads, faster query.
