# --- Stage 1: Build ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

RUN dotnet workload install wasm-tools

# Copy project file and restore dependencies first (layer-cache friendly)
COPY ["CSE325-Team-2.csproj", "./"]
RUN dotnet restore "CSE325-Team-2.csproj"

# Copy the rest of the source and publish a release build
COPY . .
RUN dotnet publish "CSE325-Team-2.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
  && echo "=== SEARCHING FOR blazor.web.js ===" \
  && find /app/publish -name "blazor.web.js" || echo "FILE NOT FOUND" \
  && echo "=== wwwroot contents ===" \
  && find /app/publish/wwwroot -type f | sort || echo "wwwroot NOT FOUND"

# --- Stage 2: Runtime --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Expose default port; Render overrides via $PORT at runtime
EXPOSE 8080

# Shell form so ${PORT:-8080} expands correctly at container start
ENTRYPOINT ["sh", "-c", "dotnet CSE325-Team-2.dll --urls http://+:${PORT:-8080}"]
