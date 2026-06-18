FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["CSE325-Team-2.csproj", "./"]
RUN dotnet restore "CSE325-Team-2.csproj"

COPY . .

# Separate publish from debug so failures are loud
RUN dotnet publish "CSE325-Team-2.csproj" \
    -c Release \
    -o /app/publish

# Verify output exists before final stage picks it up
RUN ls /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "dotnet CSE325-Team-2.dll --urls http://+:${PORT:-8080}"]