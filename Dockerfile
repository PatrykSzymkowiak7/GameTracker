# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["GameTracker/GameTracker.Api.csproj", "GameTracker/"]
COPY ["Application/GameTracker.Application.csproj", "Application/"]
COPY ["Domain/GameTracker.Domain.csproj", "Domain/"]
COPY ["Infrastructure/GameTracker.Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "GameTracker/GameTracker.Api.csproj"

COPY . .

RUN dotnet publish "GameTracker/GameTracker.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GameTracker.Api.dll"]