# SQL Server Locking and Isolation Levels (SQL)

When two transactions touch the same rows at the same time, SQL Server uses locks to keep the data consistent. A lock is a claim on a resource — a row, a page, or a table — that blocks other transactions from doing something incompatible with it. Which locks are taken, and how long they are held, depends on the transaction's isolation level.

## Shared and Exclusive Locks

A shared lock (S) is taken when a transaction reads data. Many transactions can hold shared locks on the same row at once, because reading does not change anything. An exclusive lock (X) is taken when a transaction writes data. Only one transaction can hold an exclusive lock on a row, and no shared lock can coexist with it — readers wait until the writer commits or rolls back.

```sql
-- This UPDATE takes an exclusive lock on the affected rows
-- until the transaction commits.
BEGIN TRANSACTION;
UPDATE Products SET Stock = Stock - 1 WHERE Id = 42;
COMMIT;
```

Lock escalation is the engine's way of saving memory: if one transaction takes many row locks on the same table, SQL Server may replace them with a single table lock. That is cheaper, but it blocks the whole table.

## The Read Problems

Three classic problems appear when transactions run concurrently:

- **Dirty read** — reading data written by a transaction that later rolls back. The data never really existed.
- **Non-repeatable read** — reading the same row twice in one transaction and getting different values, because another transaction changed it in between.
- **Phantom read** — running the same range query twice and getting different rows, because another transaction inserted or deleted a matching row in between.

## Isolation Levels

Each level trades consistency for concurrency:

- **Read uncommitted** — no shared locks are taken. Dirty reads, non-repeatable reads, and phantom reads are all possible. Fastest, least safe.
- **Read committed** (the default) — shared locks are held only while a row is being read, then released. Dirty reads are prevented; the other two problems remain.
- **Repeatable read** — shared locks are held until the transaction ends, so a row read twice cannot change. Phantom reads are still possible.
- **Serializable** — the strictest level. Range locks prevent new rows from appearing in a range already read, so phantom reads are prevented too. Deadlocks become much more likely.
- **Snapshot** — readers see a consistent version of the data as of the start of the transaction, using row versions in tempdb instead of locks. Writers do not block readers, and readers do not block writers.

```sql
-- Set the isolation level for the current session.
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

BEGIN TRANSACTION;
SELECT Balance FROM Accounts WHERE Id = 7;
-- The shared lock on this row is held until COMMIT.
SELECT Balance FROM Accounts WHERE Id = 7;
COMMIT;
```

## The NOLOCK Caution

The `WITH (NOLOCK)` hint (or `READUNCOMMITTED`) tells SQL Server to read without taking shared locks. It is the same as read uncommitted for that query: fast, but it can return dirty data, skip rows, or even read the same row twice while a page split moves it. It is acceptable for rough reporting or debugging, but never for anything that moves money or makes a decision based on exact values.

```sql
-- Fast but unsafe: may read uncommitted or duplicated rows.
SELECT COUNT(*) FROM Orders WITH (NOLOCK);
```

## Choosing a Level

Start with read committed, the default, and only move up when a real problem forces it. Use repeatable read or serializable for short transactions that must see a stable snapshot, such as checking a balance before deducting from it. Use snapshot isolation for read-heavy workloads where writers must never block readers — but remember it needs row versioning enabled on the database and adds tempdb pressure. Whatever the level, keep transactions short: long transactions hold locks longer and are the most common cause of blocking.
A short transaction that does one job and commits is the simplest fix for most blocking problems.
