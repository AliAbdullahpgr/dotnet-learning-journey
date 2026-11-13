# Reflection Basics in C# (C#)

Reflection lets a program inspect its own types at runtime: list the methods of a class, read its attributes, or call a method whose name is only known as a string. It is powerful, and it is also slow and fragile — use it where it earns its cost.

## Getting type information

`typeof` works on a type name at compile time; `GetType()` works on an object at runtime.

```csharp
Type stringType = typeof(string);
Type runtimeType = "hello".GetType(); // also System.String

Console.WriteLine(stringType.FullName);      // System.String
Console.WriteLine(stringType.IsClass);       // True
Console.WriteLine(stringType.Assembly);      // System.Private.CoreLib
```

## Inspecting members

```csharp
Type type = typeof(DateTime);

// All public instance methods
foreach (MethodInfo method in type.GetMethods())
    Console.WriteLine(method.Name);

// All public properties
foreach (PropertyInfo property in type.GetProperties())
    Console.WriteLine($"{property.Name} : {property.PropertyType.Name}");

// A specific method, including non-public ones
MethodInfo? addDays = type.GetMethod("AddDays");
MethodInfo? privateMethod = type.GetMethod("SomePrivateMethod",
    BindingFlags.NonPublic | BindingFlags.Instance);
```

`GetMethods()` without flags returns only public instance and static methods. Use `BindingFlags` to include private, static, or inherited members.

## Reading attributes

Attributes are metadata attached to code. Reflection is how frameworks read them — for example, how ASP.NET Core finds `[HttpGet]` or how a JSON serializer finds `[JsonIgnore]`.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class TableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[Table("Customers")]
public class Customer { public string Name { get; set; } = string.Empty; }

var attribute = typeof(Customer).GetCustomAttribute<TableAttribute>();
Console.WriteLine(attribute?.Name); // Customers
```

## Creating objects and invoking methods

```csharp
// Create an instance from a Type
object instance = Activator.CreateInstance(typeof(Customer))!;

// Call a method by name
MethodInfo? method = typeof(Customer).GetMethod("SomeMethod");
method?.Invoke(instance, new object[] { "argument" });

// Set a property by name
PropertyInfo? nameProp = typeof(Customer).GetProperty("Name");
nameProp?.SetValue(instance, "Daim");
```

`Invoke` throws `TargetInvocationException` wrapping the real exception, so error handling is awkward. Prefer compiling an expression or a delegate when the call is repeated.

## Performance cost and when to avoid it

- Reflection is roughly 10–100x slower than a direct call: member lookup, type checks, and boxing of value-type arguments all add up.
- It bypasses compile-time safety: a renamed method breaks at runtime instead of at build time.
- It can keep types alive and defeat trimming in published apps (Native AOT in particular).

Use reflection for plugins, serializers, ORMs, and test frameworks — places where the types are genuinely unknown at compile time. Avoid it as a shortcut for ordinary code: an interface, a delegate, or a `switch` on a known set of types is faster and safer.
