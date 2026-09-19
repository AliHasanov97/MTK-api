# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Directory.Packages.props", "./"]
COPY ["src/Apps/MTK.Api/MTK.Api.csproj", "src/Apps/MTK.Api/"]
COPY ["src/Common/MTK.Common.Application/MTK.Common.Application.csproj", "src/Common/MTK.Common.Application/"]
COPY ["src/Common/MTK.Common.Domain/MTK.Common.Domain.csproj", "src/Common/MTK.Common.Domain/"]
COPY ["src/Common/MTK.Common.Infrastructure/MTK.Common.Infrastructure.csproj", "src/Common/MTK.Common.Infrastructure/"]
COPY ["src/Common/MTK.Common.Presentation/MTK.Common.Presentation.csproj", "src/Common/MTK.Common.Presentation/"]
COPY ["src/Modules/Identity/MTK.Modules.Identity.Domain/MTK.Modules.Identity.Domain.csproj", "src/Modules/Identity/MTK.Modules.Identity.Domain/"]
COPY ["src/Modules/Identity/MTK.Modules.Identity.Application/MTK.Modules.Identity.Application.csproj", "src/Modules/Identity/MTK.Modules.Identity.Application/"]
COPY ["src/Modules/Identity/MTK.Modules.Identity.Infrastructure/MTK.Modules.Identity.Infrastructure.csproj", "src/Modules/Identity/MTK.Modules.Identity.Infrastructure/"]
COPY ["src/Modules/Identity/MTK.Modules.Identity.IntegrationEvents/MTK.Modules.Identity.IntegrationEvents.csproj", "src/Modules/Identity/MTK.Modules.Identity.IntegrationEvents/"]
COPY ["src/Modules/Identity/MTK.Modules.Identity.Presentation/MTK.Modules.Identity.Presentation.csproj", "src/Modules/Identity/MTK.Modules.Identity.Presentation/"]

# Restore dependencies
RUN dotnet restore "src/Apps/MTK.Api/MTK.Api.csproj"

# Copy everything else
COPY . .

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
