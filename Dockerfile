FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CSE325-Team-2.csproj", "./"]
RUN dotnet restore "CSE325-Team-2.csproj"

COPY . .
RUN dotnet publish "CSE325-Team-2.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "dotnet CSE325-Team-2.dll --urls http://+:${PORT:-8080}"]