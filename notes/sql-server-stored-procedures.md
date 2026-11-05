# SQL Server Stored Procedures

A stored procedure is a named batch of SQL saved in the database. You call it by name instead of sending the SQL text each time.

## Creating one

```sql
CREATE PROCEDURE dbo.GetOrdersByCustomer
    @CustomerId INT,
    @MinTotal DECIMAL(10, 2) = 0      -- default value: parameter is optional
AS
BEGIN
    SET NOCOUNT ON;

    SELECT OrderId, OrderDate, TotalAmount
    FROM dbo.Orders
    WHERE CustomerId = @CustomerId
      AND TotalAmount >= @MinTotal
    ORDER BY OrderDate DESC;
END;
GO
```

Call it with:

```sql
EXEC dbo.GetOrdersByCustomer @CustomerId = 42;
EXEC dbo.GetOrdersByCustomer @CustomerId = 42, @MinTotal = 100.00;
```

## Output parameters

```sql
CREATE PROCEDURE dbo.GetCustomerOrderStats
    @CustomerId INT,
    @OrderCount INT OUTPUT,
    @TotalSpent DECIMAL(10, 2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @OrderCount = COUNT(*),
           @TotalSpent = ISNULL(SUM(TotalAmount), 0)
    FROM dbo.Orders
    WHERE CustomerId = @CustomerId;
END;
GO

DECLARE @Count INT, @Spent DECIMAL(10, 2);
EXEC dbo.GetCustomerOrderStats @CustomerId = 42,
    @OrderCount = @Count OUTPUT,
    @TotalSpent = @Spent OUTPUT;
SELECT @Count AS OrderCount, @Spent AS TotalSpent;
```

## Why use them?

- **Reuse** — one definition, called from many places; change the logic once.
- **Security** — grant `EXECUTE` on the procedure without giving direct table access.
- **Performance** — execution plans are cached and reused; less SQL text travels over the network.
- **Parameters prevent SQL injection** — values are passed as typed parameters, not concatenated into SQL text.

## Key points

- `SET NOCOUNT ON` stops the "(n rows affected)" messages, which keeps result sets clean for applications.
- Use `CREATE OR ALTER PROCEDURE` to update an existing procedure without dropping it first.
- Prefer procedures for repeated, parameterised queries; keep one-off ad-hoc queries as plain SQL.
- Avoid `SELECT *` inside procedures — list the columns the caller needs.

*Learning log — 5 Nov 2026*
