# Blazor Basics (ASP.NET Core)

Blazor lets you build interactive web UIs in C# instead of JavaScript. Components are `.razor` files that mix HTML markup with C# code, and they render in the browser or on the server.

## Blazor Server vs Blazor WebAssembly

| | Blazor Server | Blazor WebAssembly |
|---|---|---|
| Where code runs | On the server | In the browser (WASM) |
| First load | Fast (small download) | Slower (downloads the .NET runtime) |
| UI updates | Over a SignalR connection | In-process, no round trip |
| Offline | No — needs the connection | Yes, after first load |
| Server load | Higher (one circuit per user) | Lower (static file hosting) |

Blazor Server fits internal tools and dashboards. WebAssembly fits public apps that must work offline or scale cheaply. A Blazor Web App can mix both with per-page render modes.

## Components

A component is a reusable piece of UI. This counter component shows the core ideas: markup, a `@code` block, data binding, and event handling.

```razor
<h3>Counter</h3>

<p>Current count: @count</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int count = 0;

    private void IncrementCount()
    {
        count++;
    }
}
```

Clicking the button calls `IncrementCount`, changes `count`, and Blazor re-renders only the parts of the page that changed.

## Data binding

One-way binding displays a value; two-way binding keeps a value and an input in sync.

```razor
<input @bind="name" placeholder="Your name" />
<p>Hello, @name!</p>

@code {
    private string name = string.Empty;
}
```

`@bind` updates `name` as the user types (on the `onchange` event by default; use `@bind:event="oninput"` for every keystroke).

## Parameters and events

Components receive data through parameters and send data back through event callbacks.

```razor
@* Child component: ProductCard.razor *@
<div class="card">
    <h4>@ProductName</h4>
    <p>@Price.ToString("C")</p>
    <button @onclick="() => OnAddToCart.InvokeAsync(ProductName)">Add to cart</button>
</div>

@code {
    [Parameter] public string ProductName { get; set; } = string.Empty;
    [Parameter] public decimal Price { get; set; }
    [Parameter] public EventCallback<string> OnAddToCart { get; set; }
}

@* Parent usage *@
<ProductCard ProductName="Keyboard" Price="49.99" OnAddToCart="HandleAdd" />
```

## When to choose Blazor

- Your team is stronger in C# than in JavaScript — one language, one toolchain, shared models and validation between client and server.
- The app is a line-of-business tool, admin panel, or dashboard rather than a public SEO-critical site.
- You need real-time UI updates (Blazor Server already runs over SignalR).

Blazor is a weaker fit for public marketing pages that need fast first paint and search indexing — there, server-rendered pages or a static site are usually better.
