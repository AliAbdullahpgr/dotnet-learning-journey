# gRPC Basics in .NET

gRPC is a high-performance RPC framework from Google. Instead of JSON over
HTTP, services exchange small binary messages defined in a `.proto` file,
using HTTP/2. It is the right choice for fast service-to-service calls
inside a system; REST with JSON stays the right choice for public APIs and
browsers.

## Why gRPC

- **Binary protobuf messages** are smaller and faster to serialize than JSON.
- **HTTP/2** gives multiplexing, header compression and bidirectional streaming.
- **A shared contract** (`.proto`) generates the client and server code, so
  both sides always agree on the shape of the data.
- **Streaming** is built in: server streaming, client streaming and
  two-way streaming.

## Define the service in a `.proto` file

```proto
syntax = "proto3";

option csharp_namespace = "GreeterService";

package greeter;

service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
  rpc StreamUpdates (HelloRequest) returns (stream HelloReply);
}

message HelloRequest {
  string name = 1;
}

message HelloReply {
  string message = 1;
}
```

## Implement the server

Add the `Grpc.AspNetCore` package, reference the proto file in the project,
and implement the generated base class:

```csharp
public class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(
        HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
```

Map it in `Program.cs`:

```csharp
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<GreeterService>();
```

## Call it from a .NET client

```csharp
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);

HelloReply reply = await client.SayHelloAsync(
    new HelloRequest { Name = "Daim" });
Console.WriteLine(reply.Message);
```

## Streaming

Streaming is where gRPC beats REST: the server can push many replies for
one request, or both sides can send a continuous stream. A live price feed
or a chat stream is a few lines of code instead of polling or WebSockets
plumbing.

## When to use gRPC — and when not to

- Use it for internal microservices, real-time data feeds, and
  polyglot systems where the `.proto` contract keeps teams in sync.
- Do not use it for browser clients (browsers cannot speak gRPC
  natively; use gRPC-Web or a REST gateway instead) or for public APIs
  that third parties consume — JSON over HTTPS is easier for them.
- gRPC needs HTTP/2 end to end; some older proxies and load balancers
  only speak HTTP/1.1, so check the infrastructure first.
