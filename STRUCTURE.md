# MTK - Proyekt Strukturu

✅ **TAMAMLANDI** - Fivestar-api strukturuna uyğun yeniləndi.

## 📁 Tam Struktur

```
MTK/
├── src/
│   ├── Apps/                                    # Host applications
│   │   └── MTK.Api/
│   │
│   ├── Common/                                  # Shared Kernel
│   │   ├── MTK.Common.Application/             # Common application logic
│   │   ├── MTK.Common.Domain/                  # Common domain logic
│   │   ├── MTK.Common.Infrastructure/          # Common infrastructure
│   │   └── MTK.Common.Presentation/            # Common presentation (base controllers)
│   │
│   └── Modules/                                 # Business Modules
│       ├── Properties/
│       │   ├── MTK.Modules.Properties.Application/
│       │   ├── MTK.Modules.Properties.Domain/
│       │   ├── MTK.Modules.Properties.Infrastructure/
│       │   ├── MTK.Modules.Properties.IntegrationEvents/
│       │   └── MTK.Modules.Properties.Presentation/
│       │
│       ├── Tenants/
│       │   ├── MTK.Modules.Tenants.Application/
│       │   ├── MTK.Modules.Tenants.Domain/
│       │   ├── MTK.Modules.Tenants.Infrastructure/
│       │   ├── MTK.Modules.Tenants.IntegrationEvents/
│       │   └── MTK.Modules.Tenants.Presentation/
│       │
│       ├── Payments/
│       │   ├── MTK.Modules.Payments.Application/
│       │   ├── MTK.Modules.Payments.Domain/
│       │   ├── MTK.Modules.Payments.Infrastructure/
│       │   ├── MTK.Modules.Payments.IntegrationEvents/
│       │   └── MTK.Modules.Payments.Presentation/
│       │
│       └── Services/
│           ├── MTK.Modules.Services.Application/
│           ├── MTK.Modules.Services.Domain/
│           ├── MTK.Modules.Services.Infrastructure/
│           ├── MTK.Modules.Services.IntegrationEvents/
│           └── MTK.Modules.Services.Presentation/
│
├── tests/                                        # Test projects
│   ├── MTK.Modules.Properties.Tests/
│   ├── MTK.Modules.Tenants.Tests/
│   └── ...
│
├── docs/                                        # Documentation
│
├── MTK.sln
└── Directory.Packages.props                     # Central Package Management
```

## 🔧 Hər Modul Strukturu

### Properties Module (Nümunə)

```
src/Modules/Properties/
│
├── MTK.Modules.Properties.Application/
│   ├── Abstractions/                  # Interfaces, base classes
│   ├── Properties/                    # CQRS operations
│   │   ├── CreateProperty/
│   │   │   ├── CreatePropertyCommand.cs
│   │   │   ├── CreatePropertyCommandHandler.cs
│   │   │   └── CreatePropertyCommandValidator.cs
│   │   ├── UpdateProperty/
│   │   ├── GetProperty/
│   │   └── GetProperties/
│   ├── Mappings/                      # AutoMapper profiles
│   └── DTOs/                          # Data Transfer Objects
│
├── MTK.Modules.Properties.Domain/
│   ├── Properties/                    # Aggregate
│   │   ├── Property.cs               # Aggregate Root
│   │   ├── PropertyCreatedDomainEvent.cs
│   │   ├── PropertyUpdatedDomainEvent.cs
│   │   └── IPropertyRepository.cs
│   ├── ValueObjects/                  # Module-specific value objects
│   └── Errors/                        # Domain errors
│
├── MTK.Modules.Properties.Infrastructure/
│   ├── Database/
│   │   ├── PropertiesDbContext.cs
│   │   ├── Configurations/
│   │   │   └── PropertyConfiguration.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   └── PropertyRepository.cs
│   ├── Outbox/                        # Outbox pattern for domain events
│   ├── Inbox/                         # Inbox pattern for integration events
│   └── PropertiesModuleInfrastructure.cs  # DI registration
│
├── MTK.Modules.Properties.IntegrationEvents/
│   ├── PropertyCreatedIntegrationEvent.cs
│   └── PropertyUpdatedIntegrationEvent.cs
│
└── MTK.Modules.Properties.Presentation/
    ├── Controllers/
    │   └── PropertiesController.cs
    ├── IntegrationEventHandlers/      # Handlers for other modules' events
    └── PropertiesModulePresentation.cs  # DI registration
```

## 🔗 Dependency Flow

```
                MTK.Common.Domain
                       ↑
        ┌──────────────┼──────────────┐
        │              │              │
MTK.Modules.*.Domain   │   MTK.Common.Application
        ↑              │              ↑
        │              │              │
MTK.Modules.*.Application ←──────────┘
        ↑
        │
MTK.Modules.*.Infrastructure
        ↑
        ├── MTK.Common.Infrastructure
        │
MTK.Modules.*.Presentation
        ↑
        ├── MTK.Common.Presentation
        │
        └─→ MTK.Api (Host)
```

## 📦 NuGet Packages (Central Management)

### Directory.Packages.props

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <!-- Application -->
    <PackageVersion Include="AutoMapper" Version="12.0.1" />
    <PackageVersion Include="FluentValidation" Version="11.12.0" />
    <PackageVersion Include="MediatR" Version="14.2.0" />

    <!-- Infrastructure -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />

    <!-- Presentation -->
    <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
</Project>
```

## 🎯 Necə İstifadə Edilir

### 1. Yeni Modul Yaratmaq

```bash
# Domain
dotnet new classlib -n MTK.Modules.{Module}.Domain

# Application
dotnet new classlib -n MTK.Modules.{Module}.Application

# Infrastructure
dotnet new classlib -n MTK.Modules.{Module}.Infrastructure

# IntegrationEvents
dotnet new classlib -n MTK.Modules.{Module}.IntegrationEvents

# Presentation
dotnet new classlib -n MTK.Modules.{Module}.Presentation
```

### 2. Solution-a Əlavə Etmək

```bash
dotnet sln add src/Modules/{Module}/**/*.csproj
```

### 3. Dependencies Əlavə Etmək

**Domain → heç nəyə depend etmir (yalnız Common.Domain)**

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Common\MTK.Common.Domain\MTK.Common.Domain.csproj" />
</ItemGroup>
```

**Application → Domain + Common.Application**

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Common\MTK.Common.Application\MTK.Common.Application.csproj" />
  <ProjectReference Include="..\MTK.Modules.{Module}.Domain\MTK.Modules.{Module}.Domain.csproj" />
</ItemGroup>
```

**Infrastructure → Application + Common.Infrastructure**

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Common\MTK.Common.Infrastructure\MTK.Common.Infrastructure.csproj" />
  <ProjectReference Include="..\MTK.Modules.{Module}.Application\MTK.Modules.{Module}.Application.csproj" />
</ItemGroup>
```

**Presentation → Infrastructure + Common.Presentation**

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Common\MTK.Common.Presentation\MTK.Common.Presentation.csproj" />
  <ProjectReference Include="..\MTK.Modules.{Module}.Infrastructure\MTK.Modules.{Module}.Infrastructure.csproj" />
</ItemGroup>
```

### 4. API-də Register Etmək

```csharp
// Program.cs
using MTK.Modules.Properties.Presentation;
using MTK.Modules.Tenants.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Register modules
builder.Services.AddPropertiesModule(builder.Configuration);
builder.Services.AddTenantsModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);
builder.Services.AddServicesModule(builder.Configuration);
```

## 🔄 Inter-Module Communication

### Domain Events → Integration Events

**Domain Event (daxili)**
```csharp
// Properties.Domain
public sealed record PropertyCreatedDomainEvent(Guid PropertyId) : IDomainEvent;
```

**Integration Event (modullar arası)**
```csharp
// Properties.IntegrationEvents
public sealed record PropertyCreatedIntegrationEvent(Guid PropertyId, DateTime OccurredOnUtc);
```

**Event Publisher**
```csharp
// Properties.Infrastructure (Outbox Pattern)
public class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(...)
    {
        // Convert domain events to integration events
        // Save to outbox table
    }
}
```

**Event Consumer**
```csharp
// Payments.Presentation
public class PropertyCreatedIntegrationEventHandler
    : IntegrationEventHandler<PropertyCreatedIntegrationEvent>
{
    public async Task Handle(PropertyCreatedIntegrationEvent @event)
    {
        // Create payment account for new property
    }
}
```

## 📊 Database Schemas

Hər modulun öz schema-sı:

```sql
-- Properties Module
properties.Properties
properties.Outbox
properties.Inbox

-- Tenants Module
tenants.Tenants
tenants.Outbox
tenants.Inbox

-- Payments Module
payments.Payments
payments.Transactions
payments.Outbox
payments.Inbox
```

## ✅ Advantages

1. **Clear Separation**: Hər modul müstəqil
2. **Scalability**: Modulları microservice-lərə çevirmək asan
3. **Team Autonomy**: Hər team öz modulunda işləyə bilər
4. **Testability**: Hər modul ayrıca test olunur
5. **Deployment**: Modulları ayrıca deploy etmək mümkün

## ✅ Migration Tamamlandı

### Current State
```
✅ src/Apps/MTK.Api/ - Host application
✅ src/Common/ - 4 layer (Domain, Application, Infrastructure, Presentation)
✅ src/Modules/Properties/ - 5 layer (tam nümunə)
✅ Directory.Packages.props - Central package management
✅ Solution faylı - Fivestar strukturu ilə
```

### Completed Steps

1. ✅ Fivestar strukturu yaradıldı
2. ✅ Common 4 proyektə bölündü
3. ✅ Properties modulu 5 proyektə bölündü
4. ✅ API src/Apps-a köçürüldü
5. ✅ Central package management əlavə edildi
6. ✅ Build uğurla tamamlandı
7. ✅ Köhnə proyektlər silindi

### Next Steps

1. ⏳ Tenants modulu implement et
2. ⏳ Payments modulu implement et
3. ⏳ Services modulu implement et
4. ⏳ Integration Events əlavə et
5. ⏳ Outbox/Inbox pattern implement et

Struktur tam hazırdır! 🎉
