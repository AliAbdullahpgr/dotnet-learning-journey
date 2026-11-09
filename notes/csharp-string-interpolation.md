# String Interpolation in C#

String interpolation builds a string by embedding expressions directly inside a string literal, using `$` before the opening quote and `{ ... }` around each expression.

## Key points

- Prefix the string with `$`, for example `$"Hello, {name}"`.
- Any valid expression can go inside the braces: a variable, a method call, or arithmetic such as `{price * quantity}`.
- Add a format after a colon, for example `{price:C2}` for currency or `{date:yyyy-MM-dd}` for a date.
- Add alignment after a comma, for example `{name,-10}` left-aligns the value in a field 10 characters wide.
- To include a literal brace, double it: `{{` and `}}`.
- Interpolated strings are clearer and less error-prone than manual concatenation, and the compiler checks the expressions at compile time.
- For culture-sensitive output, combine interpolation with `CultureInfo` or `FormattableString` when the default culture is not what you want.

## Example

```csharp
string name = "Daim";
decimal price = 19.99m;
int quantity = 3;

string message = $"Hello, {name}! {quantity} items cost {price * quantity:C2}.";
Console.WriteLine(message);
// Output (en-US culture): Hello, Daim! 3 items cost $59.97.
```
