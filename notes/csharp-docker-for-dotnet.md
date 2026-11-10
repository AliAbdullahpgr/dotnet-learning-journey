# Docker for .NET Apps

Docker packages a .NET app with its runtime into a container: the same
image runs on your machine, on a server, and in the cloud. No more "it
works on my machine".

## A multi-stage Dockerfile

Multi-stage builds keep the final image small: build with the SDK image,
run with the smaller ASP.NET runtime image.

```dockerfile
# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyApi.dll"]
```

Build and run:

```bash
docker build -t myapi:1.0 .
docker run -p 8080:8080 myapi:1.0
```

The API is now at `http://localhost:8080`.

## Why two stages

- The SDK image is large (it contains compilers). The runtime image is
  much smaller and has no build tools — a smaller attack surface and
  faster deploys.
- Copying the `.csproj` and restoring *before* copying the source means
  Docker caches the restore layer: rebuilding after a code change skips
  the restore step.

## `.dockerignore`

Keep the build context small, like `.gitignore` for Docker:

```
bin/
obj/
.git/
*.md
```

Without it, Docker sends your `bin`, `obj` and `.git` folders into the
build every time.

## Docker Compose for app + database

```yaml
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__Default=Server=db;Database=Shop;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
    depends_on:
      - db
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
```

```bash
docker compose up
```

## Rules to remember

- Never run as root in production: add `USER app` in the final stage
  (the ASP.NET image already has an `app` user).
- Do not bake secrets into the image: pass them with environment
  variables or a secret store at run time.
- Pin image tags (`aspnet:8.0`, not `aspnet:latest`) so a rebuild next
  month does not silently change the runtime.
- Health checks matter: expose the ASP.NET Core health-check endpoint
  and let Docker restart unhealthy containers.
