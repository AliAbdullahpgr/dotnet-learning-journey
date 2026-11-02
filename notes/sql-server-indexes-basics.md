# SQL Server Index Basics

An index helps SQL Server find rows without scanning the whole table, much like an index in a book.

## Clustered vs nonclustered

- A clustered index sorts and stores the table rows themselves. A table has at most one clustered index, usually on the primary key.
- A nonclustered index stores the key values plus a pointer to the row. A table can have many nonclustered indexes.

## Example

```sql
-- Speed up lookups by CustomerId
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId
ON Orders (CustomerId);

-- Cover a frequent query by including extra columns
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId_OrderDate
ON Orders (CustomerId)
INCLUDE (OrderDate, TotalAmount);
```

## When to index

- Columns used in WHERE, JOIN, and ORDER BY clauses.
- Foreign keys, which are often joined on.

## Watch out

- Every index slows down INSERT, UPDATE, and DELETE a little, because the index must be maintained too.
- Do not index every column: a few well-chosen indexes beat many unused ones.
