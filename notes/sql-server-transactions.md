# SQL Server Transactions

## What is a transaction?

A transaction groups several statements into one atomic unit: either
all of them succeed, or none of them does. The classic example is a
money transfer — the debit and the credit must happen together.

```sql
BEGIN TRANSACTION;

UPDATE Accounts SET Balance = Balance - 100 WHERE AccountId = 1;
UPDATE Accounts SET Balance = Balance + 100 WHERE AccountId = 2;

COMMIT TRANSACTION;
```

If anything fails in between, `ROLLBACK TRANSACTION` undoes every
change made since `BEGIN TRANSACTION`.

## ACID

- **Atomicity** — all or nothing.
- **Consistency** — the database moves from one valid state to another.
- **Isolation** — concurrent transactions do not see each other's
  half-finished work.
- **Durability** — once committed, the change survives a crash.

## Safe pattern with TRY / CATCH

```sql
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE Accounts SET Balance = Balance - 100 WHERE AccountId = 1;
    UPDATE Accounts SET Balance = Balance + 100 WHERE AccountId = 2;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH
```

## Isolation levels

The isolation level decides which concurrency problems a transaction
can see:

| Level | Dirty read | Non-repeatable read | Phantom read |
|---|:---:|:---:|:---:|
| Read Uncommitted | Yes | Yes | Yes |
| Read Committed (default) | No | Yes | Yes |
| Repeatable Read | No | No | Yes |
| Serializable | No | No | No |
| Snapshot | No | No | No |

- **Dirty read** — reading data from a transaction that later rolls back.
- **Non-repeatable read** — the same row returns different values
  when read twice in one transaction.
- **Phantom read** — a query returns different sets of rows when run
  twice, because another transaction inserted rows.

```sql
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;
-- statements here
COMMIT TRANSACTION;
```

## Things to remember

- Keep transactions short: long ones hold locks and block others.
- `Read Committed` is the default and is right for most work.
- Higher isolation means more correctness but more blocking — pick
  the lowest level that is still correct for the job.
- `Snapshot` isolation avoids most blocking but needs version store
  space in tempdb, so it must be enabled on the database first.
