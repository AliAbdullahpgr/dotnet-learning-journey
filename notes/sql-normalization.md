# Database Normalization

Normalization organizes a relational database so each fact is stored once, in one place. It reduces redundant data and the update, insert, and delete anomalies that redundancy causes.

## Key points

- **First normal form (1NF):** every column holds atomic values (no lists or repeating groups), and each row is unique, identified by a primary key.
- **Second normal form (2NF):** in 1NF, and every non-key column depends on the *whole* primary key — no partial dependencies on part of a composite key.
- **Third normal form (3NF):** in 2NF, and no transitive dependencies — non-key columns depend only on the key, not on other non-key columns.
- **Boyce-Codd normal form (BCNF):** a stricter version of 3NF — every determinant (anything other columns depend on) must be a candidate key.
- Normalizing further usually means more joins at read time, so heavily normalized schemas can be slower for reporting; teams sometimes denormalize deliberately for performance, accepting the extra care that requires.

## Example

```sql
-- Before: customer details repeat on every order row
-- Orders(OrderId, CustomerName, CustomerAddress, OrderDate)

-- After: customer data lives in one table, referenced by key
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Address NVARCHAR(200)
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    OrderDate DATE
);
```

Changing a customer's address now updates one row in `Customers` instead of every one of their orders.
