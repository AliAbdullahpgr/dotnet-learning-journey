# C# Source Generators

A source generator runs at compile time and writes extra C# code into
your compilation. The generated code is real, compiled code — there is
no reflection and no runtime cost. Generators ship as analyzers and are
referenced like any other package.

## Why generate code at compile time

- **No runtime reflection** — serialization, logging and mapping code
  is generated once at build time instead of discovered with reflection
  on every startup.
- **Fewer handwritten repetitions** — boilerplate like `ToString`,
  equality members or DTO mapping is produced from a single attribute.
- **Compile-time errors** — a generator can report a diagnostic when
  your code is wrong, instead of failing at runtime.

## A minimal generator

A generator implements `IIncrementalGenerator` and registers a
pipeline that reads syntax and emits source:

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

[Generator]
public sealed class HelloGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find every class marked with [GenerateHello].
        var classes = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "Demo.GenerateHelloAttribute",
                predicate: static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax,
                transform: static (ctx, _) => ((Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax)ctx.TargetNode).Identifier.Text);

        context.RegisterSourceOutput(classes, static (ctx, className) =>
        {
            string source = $$"""
                namespace Demo;

                public static partial class {{className}}Hello
                {
                    public static string SayHello() => "Hello from a source generator!";
                }
                """;
            ctx.AddSource($"{className}Hello.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }
}
```

The generator project must target `netstandard2.0` and reference the
`Microsoft.CodeAnalysis` packages. It is consumed as an analyzer
reference, not as a normal project reference.

## Generators you already use

- **System.Text.Json** — `[JsonSerializable]` generates the
  serializer for a type, so trimming and Native AOT builds work.
- **Regular expressions** — `[GeneratedRegex("...")]` compiles the
  pattern at build time instead of at first use.
- **Logging** — `[LoggerMessage]` generates fast, allocation-free
  logging methods.

## Rules and pitfalls

- A generator can only *add* source files; it can never change the
  code you wrote. Generate a `partial` class and let your code fill
  in the rest.
- Keep the pipeline incremental (`ForAttributeWithMetadataName`,
  `Select`, `Where`) — a generator that re-runs on every keystroke
  slows the IDE down.
- Debugging a generator is awkward: use `Debugger.Launch()` or write
  the generated text to a file, and check the emitted `.g.cs` files
  under `obj/` to see what was actually produced.
- Generated code is compiled in, so a bad generator breaks the build
  — treat generator code with the same tests and reviews as any other.

## Takeaway

Reach for a source generator when the same code would otherwise be
written by hand for every type, or when reflection is doing work that
could be decided at build time. Start with the built-in generators
(JSON, regex, logging) before writing your own.
