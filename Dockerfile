# --- Stage 1: Build ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first (layer-cache friendly)
COPY ["CSE325-Team-2.csproj", "./"]
RUN dotnet restore "CSE325-Team-2.csproj"

# Copy the rest of the source and publish a release build
COPY . .
RUN dotnet publish "CSE325-Team-2.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# --- Stage 2: Runtime --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/publish .

# Render injects PORT at runtime; ASP.NET Core reads ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the default port (Render overrides this via $PORT)
EXPOSE 8080

ENTRYPOINT ["dotnet", "CSE325_Team_2.dll"]
