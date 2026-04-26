# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["CapLed.API/CapLed.API.csproj", "CapLed.API/"]
COPY ["CapLed.Core/CapLed.Core.csproj", "CapLed.Core/"]
COPY ["CapLed.Infrastructure/CapLed.Infrastructure.csproj", "CapLed.Infrastructure/"]
RUN dotnet restore "CapLed.API/CapLed.API.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/CapLed.API"
RUN dotnet build "CapLed.API.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "CapLed.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080

# Environment variables for database should be provided by the host (Render/Railway)
# e.g. ConnectionStrings__DefaultConnection="Server=...;"

ENTRYPOINT ["dotnet", "StockManager.API.dll"]
