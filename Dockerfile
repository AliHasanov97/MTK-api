# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# The per-csproj COPY list this used to have (to let restore cache
# independently of source changes) silently went stale as modules were added —
# it only ever listed Identity's projects, so `dotnet restore` failed outright
# for a Buildings/Payments-only change that was never caught until deploy.
# Copying the whole source tree before restore trades a little layer-cache
# efficiency for never going stale like that again.
COPY . .

RUN dotnet restore "src/Apps/MTK.Api/MTK.Api.csproj"

# Build
WORKDIR "/src/src/Apps/MTK.Api"
RUN dotnet build "MTK.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "MTK.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTK.Api.dll"]
