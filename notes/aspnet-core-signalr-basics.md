# SignalR Basics in ASP.NET Core

SignalR is the ASP.NET Core library for real-time communication: the server
can push messages to connected clients the moment something happens, instead
of the client polling the API. Typical uses are chat, live notifications,
dashboards and collaborative editing.

## How it works

SignalR picks the best available transport automatically:

- **WebSockets** when the client and server both support it (the default
  and the fastest).
- **Server-Sent Events** and **Long Polling** as fallbacks.

You write a Hub on the server and call its methods from the client, and
the server calls methods on the client. The transport is an implementation
detail.

## A simple chat hub

```csharp
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // Sends to every connected client.
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToGroup(string group, string message)
    {
        await Clients.Group(group).SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "general");
        await base.OnConnectedAsync();
    }
}
```

Register SignalR and map the hub in `Program.cs`:

```csharp
builder.Services.AddSignalR();

var app = builder.Build();
app.MapHub<ChatHub>("/chathub");
```

## Calling the hub from a client

With the JavaScript client:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(user + ": " + message);
});

await connection.start();
await connection.invoke("SendMessage", "Daim", "Hello!");
```

## Targeting clients

- `Clients.All` — everyone connected.
- `Clients.Caller` — only the client that called the hub method.
- `Clients.Others` — everyone except the caller.
- `Clients.Group("name")` — members of a group.
- `Clients.User(userId)` — a specific authenticated user.

## Rules to remember

- Hub methods should be small and fast; heavy work belongs in a
  background service, which can push results through `IHubContext<T>`.
- A hub instance is created per connection and is transient — do not store
  state in hub fields. Keep shared state in a singleton service.
- For multiple servers, use a backplane (Redis) so a message sent on one
  server reaches clients connected to another.
- Always use HTTPS in production: SignalR sends the connection id and
  tokens over the wire.
