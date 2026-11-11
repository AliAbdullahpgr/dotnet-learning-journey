# SQL Index Deep Dive

An index is a sorted copy of some columns that lets SQL Server find rows
without scanning the whole table. The basics (clustered vs nonclustered)
are covered in the indexes-basics note — this one goes deeper into how
indexes actually behave.

## Clustered vs nonclustered, precisely

- A table has **one** clustered index: it *is* the table, sorted by the
  key. Pick a narrow, ever-increasing key (an `IDENTITY` int) so new rows
  append at the end instead of forcing page splits in the middle.
- Nonclustered indexes are separate structures holding the key columns
  plus a pointer (the clustered key) back to the row. A table can have
  many of them, but each one costs space and slows down every insert,
  update and delete.

## Covering indexes and included columns

A query that can be answered from the index alone never touches the
table — this is a **covering index**, and it is the fastest possible read:

```sql
-- Covers lookups by CategoryId that also need Name and Price.
CREATE INDEX IX_Products_Category_Covering
ON Products (CategoryId)
INCLUDE (Name, Price);
```

Key columns are sorted and searchable; `INCLUDE` columns are stored but
not sorted, so they cannot be used for filtering — only for covering.
Put filter columns in the key, return-only columns in `INCLUDE`.

## Filtered indexes

When a query only ever touches a subset, index just that subset:

```sql
-- Only active products; small, fast, and cheap to maintain.
CREATE INDEX IX_Products_Active
ON Products (CategoryId)
WHERE IsActive = 1;
```

The query must match the filter predicate exactly, or the optimizer
ignores the index.

## Composite index order matters

For `WHERE CategoryId = @c AND Price < @p ORDER BY CreatedAt`, the index
`(CategoryId, Price, CreatedAt)` helps; `(Price, CategoryId, CreatedAt)`
usually does not — the equality column must come first, then the range,
then the ordering columns. An index can only be used left-to-right.

## Fragmentation and maintenance

- Updates and deletes leave gaps and split pages; over time an index
  becomes **fragmented** and reads more pages than it should.
- `REORGANIZE` fixes light fragmentation cheaply; `REBUILD` rewrites the
  index and is for heavy fragmentation (and can be done online in
  Enterprise edition).
- Check fragmentation with `sys.dm_db_index_physical_stats` and only
  maintain indexes that are actually fragmented — rebuilding everything
  nightly wastes time and log space.

## How many indexes is too many?

Every index is a trade-off: reads get faster, writes get slower, storage
grows. Measure with the missing-index DMVs and `STATISTICS IO` before
adding one, and drop indexes the workload never uses
(`sys.dm_db_index_usage_stats` shows zero seeks/scans).
