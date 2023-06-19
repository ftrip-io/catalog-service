FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /build

ARG CONFIGURATION=Release

# Restore dependencies
COPY ["ftrip.io.catalog-service.contracts/*.csproj", "ftrip.io.catalog-service.contracts/package*.json", "./ftrip.io.catalog-service.contracts/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.catalog-service.contracts/ftrip.io.catalog-service.contracts.csproj

COPY ["ftrip.io.catalog-service/*.csproj", "ftrip.io.catalog-service/package*.json", "./ftrip.io.catalog-service/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.catalog-service/ftrip.io.catalog-service.csproj

# Publish
COPY . .

RUN dotnet publish -c $CONFIGURATION -o out ftrip.io.catalog-service/ftrip.io.catalog-service.csproj
 
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /app

RUN addgroup --system ftrip-io
RUN adduser --system catalog-service

COPY --from=build --chown=catalog-service:ftrip-io --chmod=500 /build/out .

EXPOSE 80

USER catalog-service

ENTRYPOINT ["dotnet", "ftrip.io.catalog-service.dll"]