# Tuples and Deconstruction in C#

A tuple groups several values of different types into one lightweight value, without defining a new class or record first.

## Key points

- Write a tuple type in parentheses, for example `(string Name, int Age)`; the elements can be named or unnamed.
- Unnamed elements are reached as `Item1`, `Item2`, and so on, but named elements such as `.Name` and `.Age` are much clearer.
- Tuples are value types (`ValueTuple`), so comparing two tuples with `==` compares their elements, not their identities.
- Deconstruction splits a tuple back into separate variables: `var (name, age) = person;`.
- Use `_` as a discard for a part you do not need: `var (name, _) = person;`.
- Tuples are ideal for returning more than one value from a method, for example `(bool Found, int Index)`.
- For data that is passed around widely or needs behaviour, prefer a record or a class instead of a tuple.

## Example

```csharp
(string Name, int Age) GetPerson() => ("Daim", 25);

var (name, age) = GetPerson();
Console.WriteLine($"{name} is {age} years old.");
// Output: Daim is 25 years old.
```
