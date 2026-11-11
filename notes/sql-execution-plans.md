# SQL Execution Plans

An execution plan is the step-by-step strategy SQL Server uses to run a
query: which indexes it reads, in what order it joins tables, and how it
sorts or filters. When a query is slow, the plan shows *where* the time
goes instead of guessing.

## Getting a plan

- In SSMS: run the query with **Include Actual Execution Plan**
  (Ctrl+M) to see what really happened, including real row counts.
- `SET STATISTICS IO ON` shows logical reads per table — the fastest way
  to spot a query that reads far more pages than it returns.
- The **estimated** plan is the optimizer's guess; the **actual** plan
  also shows where the guess was wrong, which is usually the bug.

## Operators to recognize

- **Index Seek** — jumps straight to the matching rows via the index.
  This is what you want for a selective filter.
- **Index / Table Scan** — reads every row. Fine for a tiny table or a
  query that really needs most rows; a warning sign otherwise.
- **Key Lookup** — the index found the row, but the query needs columns
  the index does not cover, so SQL Server fetches each row from the table.
  Many lookups mean the index should cover more columns.
- **Nested Loops / Hash Match / Merge Join** — three join strategies:
  nested loops for a small outer side, hash for large unsorted inputs,
  merge when both sides are already sorted.
- **Sort / Hash Aggregate** — extra work after the data is read; often
  avoidable with the right index order.

## Common red flags

- A **scan on a large table** where the `WHERE` clause filters on a
  column with no index.
- **Actual rows much larger than estimated rows** — the optimizer picked
  the wrong plan because its statistics were stale; update statistics or
  check for parameter sniffing.
- A **missing index suggestion** (green text in SSMS) — a hint, not an
  order. Verify it helps the whole workload before adding it; every index
  slows down writes.
- **Implicit conversions** — comparing a `varchar` column to an
  `nvarchar` value (or a number to a string) silently converts every row
  and kills index seeks. Match the parameter type to the column type.

## A quick routine for a slow query

1. Run it with the actual plan and `STATISTICS IO`.
2. Find the operator with the highest cost percentage.
3. Ask why: missing index, wrong index, stale statistics, or a filter the
   optimizer cannot use (a function on the column, a `LIKE '%x%'`).
4. Fix that one thing, re-run, and compare logical reads — the number
   should drop sharply.
