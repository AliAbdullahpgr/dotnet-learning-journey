# Span and Memory in C#

`Span<T>` is a window over contiguous memory — an array, a slice of an
array, or stack memory — that lets you work on data without allocating
or copying it. `Memory<T>` is its heap-friendly sibling for async code.
Both exist for one reason: **avoid allocations in hot paths**.

## Why spans matter

Ordinary slicing copies: `array.Skip(2).Take(3)` allocates, and
`Substring` allocates a new string. A span just points at the same memory
with an offset and a length:

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };
Span<int> slice = numbers.AsSpan(1, 3); // 2, 3, 4 — no copy
slice[0] = 99;                          // numbers[1] is now 99
```

## `stackalloc` and parsing

```csharp
// A small buffer on the stack — zero heap allocation.
Span<char> buffer = stackalloc char[32];
"hello".AsSpan().CopyTo(buffer);

// Parse without allocating substrings.
ReadOnlySpan<char> text = "2026-11-11".AsSpan();
ReadOnlySpan<char> year = text.Slice(0, 4);
int y = int.Parse(year); // int.Parse accepts a span — no Substring needed
```

`stackalloc` is for small, short-lived buffers only (a few hundred bytes);
large buffers belong on the heap.

## Span vs Memory

- `Span<T>` is a `ref struct`: it can only live on the stack, so it
  cannot be a field, cannot cross an `await`, and cannot be boxed. That
  restriction is what makes it provably safe — the memory it points at
  cannot outlive its frame.
- `Memory<T>` is a normal struct that can be stored in fields and passed
  through `await`. Call `.Span` on it when you need to work with the data.
  Use `Memory<T>` (and `ReadOnlyMemory<T>`) at API boundaries, `Span<T>`
  inside the method doing the work.

## Practical rules

- Prefer `ReadOnlySpan<T>` for inputs you only read — it accepts arrays,
  strings and spans alike.
- Methods like `string.AsSpan()`, `int.Parse(ReadOnlySpan<char>)` and
  `Utf8JsonWriter` are built for spans; using them removes the hidden
  allocations in parsing and formatting code.
- Do not reach for spans everywhere: they pay off in parsers, serializers
  and tight loops, not in ordinary business logic where one allocation
  per request is invisible.
