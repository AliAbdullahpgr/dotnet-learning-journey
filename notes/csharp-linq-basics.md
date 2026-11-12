# LINQ Basics in C#

LINQ (Language Integrated Query) lets you query collections in C# with
readable, type-safe code instead of writing manual loops for filtering,
sorting, and transforming data. It works on lists, arrays, and, through
providers like EF Core, on databases too.

## Two syntaxes, same idea

Query syntax looks a bit like SQL:

```csharp
var adults = from s in students
             where s.Age >= 18
             orderby s.Name
             select s.Name;
```

Method syntax uses extension methods and lambdas, and is more common
in real code:

```csharp
var adults = students
    .Where(s => s.Age >= 18)
    .OrderBy(s => s.Name)
    .Select(s => s.Name);
```

Both produce the same result. Pick one style per query and stay
consistent inside it.

## The operators you will use most

- `Where` — keeps only items matching a condition.
- `Select` — projects each item into a new shape, e.g. just a name.
- `OrderBy` / `OrderByDescending` — sorts; add `ThenBy` for a second key.
- `FirstOrDefault` — returns the first match, or null (for reference
  types) when there is none. Prefer it over `First` when a match may
  not exist, so a missing item is not an exception.
- `Any` — checks whether any item matches, without building a list.
- `Count` — counts matches; use `Any()` instead when you only need
  to know if something exists.

## Deferred execution — the key gotcha

Most LINQ queries do not run when you write them. They run when you
enumerate the result, for example with `foreach` or `ToList()`. That
means if the source list changes before enumeration, the result
changes too. Call `ToList()` when you want a snapshot taken right now:

```csharp
var names = students
    .Where(s => s.Age >= 18)
    .Select(s => s.Name)
    .ToList(); // query runs here, result is now fixed
```

## Worked example

### A deferred query versus a snapshot

This self-contained example uses LINQ to Objects:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

var numbers = new List<int> { 1, 2, 3, 4 };
var evenNumbers = numbers.Where(n => n % 2 == 0);
var snapshot = evenNumbers.ToList(); // evaluates the query now

numbers.Add(6);

Console.WriteLine(string.Join(", ", evenNumbers)); // 2, 4, 6
Console.WriteLine(string.Join(", ", snapshot));    // 2, 4

numbers.Add(8);
Console.WriteLine(string.Join(", ", evenNumbers)); // 2, 4, 6, 8
```

Enumerating `evenNumbers` again runs the query against the current list.
`snapshot` is a separate list containing the values selected earlier.
For reference types, `ToList()` copies the references; it does not clone
the objects themselves.

### Filtering and sorting students

```csharp
var students = new List<Student>
{
    new("Ayesha", 20),
    new("Bilal", 17),
    new("Sara", 22),
};

var adultNames = students
    .Where(s => s.Age >= 18)
    .OrderBy(s => s.Name)
    .Select(s => s.Name)
    .ToList();
// Result: ["Ayesha", "Sara"]
```

## Takeaway

Start with `Where`, `Select`, and `OrderBy`. Learn deferred execution
early — it explains most surprising LINQ behaviour — and reach for
`FirstOrDefault` and `Any` before writing a loop by hand.
