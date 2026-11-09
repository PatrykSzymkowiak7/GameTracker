# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/GameTracker.Api/GameTracker.Api.csproj", "src/GameTracker.Api/"]
COPY ["src/GameTracker.Application/GameTracker.Application.csproj", "src/GameTracker.Application/"]
COPY ["src/GameTracker.Domain/GameTracker.Domain.csproj", "src/GameTracker.Domain/"]
COPY ["src/GameTracker.Infrastructure/GameTracker.Infrastructure.csproj", "src/GameTracker.Infrastructure/"]

RUN dotnet restore "src/GameTracker.Api/GameTracker.Api.csproj"

COPY . .

RUN dotnet publish "src/GameTracker.Api/GameTracker.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

RUN apt-get update \
        && apt-get install -y --no-install-recommends curl \
        && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GameTracker.Api.dll"]