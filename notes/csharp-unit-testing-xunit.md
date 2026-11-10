# Unit Testing with xUnit in C#

A unit test checks one small behaviour in isolation. xUnit is the standard
test framework for .NET: tests are methods marked with attributes, and the
`dotnet test` command runs them all.

## Your first test

Add the `xunit` and `Microsoft.NET.Test.Sdk` packages to a test project,
then write a test in the Arrange–Act–Assert shape:

```csharp
using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        int result = calculator.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }
}
```

Run with `dotnet test`. A green run means the behaviour holds; a red run
tells you exactly which expectation failed.

## `[Fact]` vs `[Theory]`

- `[Fact]` — one test, one case.
- `[Theory]` — the same test with many inputs, supplied by
  `[InlineData]`:

```csharp
[Theory]
[InlineData(2, 3, 5)]
[InlineData(-1, 1, 0)]
[InlineData(0, 0, 0)]
public void Add_AnyNumbers_ReturnsSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    Assert.Equal(expected, calculator.Add(a, b));
}
```

One theory replaces three near-identical fact methods.

## Common assertions

```csharp
Assert.Equal(5, result);              // equality
Assert.True(result > 0);              // condition
Assert.Null(maybeNull);               // null check
Assert.Contains("error", message);    // string contains
Assert.Throws<ArgumentException>(     // exception expected
    () => calculator.Divide(1, 0));
Assert.Single(items);                 // collection has one item
```

## Test naming

Name a test after the behaviour, not the method: `Add_TwoPositiveNumbers_ReturnsSum`
reads as a specification. The pattern is
`MethodName_Scenario_ExpectedResult`.

## Rules to remember

- Test behaviour, not implementation: a test that breaks every time you
  refactor is a test of the internals, not of the contract.
- One logical assertion per test (a theory with several cases still counts
  as one behaviour).
- Keep tests fast and independent: no shared mutable state, no real
  database, no network. Dependencies get fakes or mocks — Moq is the
  standard mocking library for .NET.
- A test that never fails is not a test: check it fails first by breaking
  the code on purpose, then fix it. Red → green is the proof the test
  actually guards anything.
