# Unit Testing with Moq (C#)

xUnit runs the test; Moq provides the fake dependencies. A **mock** is a
stand-in object that behaves the way the test tells it to: return this
value, throw that exception, and record what was called so the test can
verify it.

## The basic pattern

```csharp
var repo = new Mock<IProductRepository>();
repo.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new Product("Keyboard", 49.99m));

var service = new ProductService(repo.Object);
var result = await service.GetNameAsync(42);

Assert.Equal("Keyboard", result);
repo.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()),
    Times.Once);
```

`Setup` programs the behaviour, `It.IsAny<T>()` matches any argument,
and `Verify` checks the call actually happened.

## Setup and Verify essentials

- `.ReturnsAsync(value)` for async methods; `.Returns(value)` for sync.
- `.ThrowsAsync(new NotFoundException())` to test error paths.
- `Times.Once`, `Times.Never`, `Times.AtLeastOnce()` control how strict
  the verification is.
- `It.Is<int>(id => id > 0)` matches only arguments passing a predicate;
  `It.IsAny<string>()` matches anything.

## Strict vs loose mocks

- **Loose** (the default) — unconfigured calls return default values
  (`null`, `0`, empty). Forgiving, but a forgotten setup fails silently
  with a `NullReferenceException` somewhere downstream.
- **Strict** (`new Mock<T>(MockBehavior.Strict)`) — any unconfigured call
  throws immediately. Noisy, but every surprise surfaces at the exact
  call that caused it. Prefer strict for interfaces with few methods.

## What to mock — and what not to

- Mock **your own abstractions**: repositories, email senders, HTTP
  clients (via `HttpMessageHandler`), clocks and id generators.
- Do **not** mock the thing under test, and do not mock concrete classes
  with heavy behaviour — test them for real instead.
- Do not mock `DbContext` directly: its surface is huge and the mocks
  become lies. Use the in-memory provider or a real test database for
  EF Core tests, and mock only the repository interface above it.

## A common mistake

Mocking everything makes the test verify the mock, not the code. If a
test needs five mocks to run, the class probably does too much — that is
a design smell, not a testing problem. Split the class, then the tests
get simple on their own.
