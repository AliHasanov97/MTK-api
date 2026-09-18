# MTK - Modular Monolith Architecture

Bu proyekt **Modular Monolith** pattern-i ilə qurulub. Hər modul müstəqil "mini-application" kimidir, amma hamısı bir prosesdə işləyir.

## 🏗️ Arxitektura

```
MTK/
├── MTK.Common/              # Shared Kernel
│   ├── Abstractions/        # Entity, Result, Error, IUnitOfWork
│   ├── ValueObjects/        # Money, Currency
│   ├── Messaging/           # CQRS interfaces
│   ├── Behaviors/           # ValidationBehavior
│   └── Exceptions/          # Custom exceptions
│
├── modules/                 # Modullar (Virtual folder)
│   ├── MTK.Modules.Properties/
│   ├── MTK.Modules.Tenants/
│   ├── MTK.Modules.Payments/
│   └── MTK.Modules.Services/
│
├── MTK.Infrastructure/      # Shared Infrastructure (optional)
└── MTK.Api/                 # Host application
```

## 📦 Modul Strukturu

Hər modul öz daxilində **Vertical Slice Architecture** istifadə edir:

```
MTK.Modules.{ModuleName}/
├── Domain/                  # Business entities, domain events
│   ├── {Entity}.cs         # Aggregate Root
│   ├── {Entity}Events.cs   # Domain Events
│   └── I{Entity}Repository.cs
│
├── Application/            # Use cases (CQRS)
│   ├── Create{Entity}/
│   │   ├── Command.cs
│   │   ├── CommandHandler.cs
│   │   └── CommandValidator.cs
│   └── Get{Entity}/
│       ├── Query.cs
│       ├── QueryHandler.cs
│       └── DTO.cs
│
├── Infrastructure/         # Data access
│   ├── {Module}DbContext.cs
│   ├── {Entity}Repository.cs
│   └── {Entity}Configuration.cs
│
└── {Module}Module.cs       # Module registration
```

## 🔗 Module Dependencies

```
MTK.Common ← Base (heç kimə depend etmir)
    ↑
    ├── MTK.Modules.Properties
    ├── MTK.Modules.Tenants
    ├── MTK.Modules.Payments
    └── MTK.Modules.Services
        ↑
        └── MTK.Api (host)
```

**Qaydalar:**
- ✅ Modullar yalnız `MTK.Common`-a depend edə bilər
- ❌ Modullar bir-birinə **direct dependency** yarada bilməz
- ✅ Modullar arasında əlaqə **Domain Events** vasitəsilə

## 🚀 Yeni Modul Yaratmaq

### 1. Proyekt Yaradın

```bash
dotnet new classlib -n MTK.Modules.{ModuleName}
```

### 2. Dependency Əlavə Edin

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
  <ProjectReference Include="..\MTK.Common\MTK.Common.csproj" />
</ItemGroup>
```

### 3. Struktur Yaradın

```
MTK.Modules.{ModuleName}/
├── Domain/
├── Application/
├── Infrastructure/
└── {ModuleName}Module.cs
```

### 4. Module Registration

```csharp
// {ModuleName}Module.cs
public static class {ModuleName}Module
{
    public static IServiceCollection Add{ModuleName}Module(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<{ModuleName}DbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Database")));

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<{ModuleName}DbContext>());

        // Repositories
        services.AddScoped<I{Entity}Repository, {Entity}Repository>();

        // MediatR Handlers
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof({ModuleName}Module).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof({ModuleName}Module).Assembly);

        return services;
    }
}
```

### 5. API-də Register Edin

```csharp
// Program.cs
builder.Services.Add{ModuleName}Module(builder.Configuration);
```

## 💬 Module Communication

Modullar bir-biri ilə **Domain Events** vasitəsilə əlaqə qurur:

### Event Publish

```csharp
// MTK.Modules.Properties/Domain/Property.cs
public static Property Create(string name, Money price)
{
    var property = new Property(...);

    // Raise domain event
    property.RaiseDomainEvent(new PropertyCreatedDomainEvent(property.Id));

    return property;
}
```

### Event Subscribe

```csharp
// MTK.Modules.Payments/Application/PropertyCreatedEventHandler.cs
public class PropertyCreatedEventHandler
    : INotificationHandler<PropertyCreatedDomainEvent>
{
    public async Task Handle(
        PropertyCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Yeni mənzil üçün payment account yaradır
    }
}
```

## 🗄️ Database Schemas

Hər modul öz schema-sını istifadə edir:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.HasDefaultSchema("properties"); // Module-specific schema
}
```

**Schemas:**
- `properties` - Properties module
- `tenants` - Tenants module
- `payments` - Payments module
- `services` - Services module

## 🎯 CQRS Pattern

Hər modülda Commands və Queries ayrılır:

### Command (Write)

```csharp
// Command
public sealed record CreatePropertyCommand(...) : ICommand<Guid>;

// Handler
public class CreatePropertyCommandHandler
    : ICommandHandler<CreatePropertyCommand, Guid>
{
    public async Task<Result<Guid>> Handle(...)
    {
        // Business logic
    }
}

// Validator
public class CreatePropertyCommandValidator
    : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```

### Query (Read)

```csharp
// Query
public sealed record GetPropertyQuery(Guid Id) : IQuery<PropertyDto>;

// Handler
public class GetPropertyQueryHandler
    : IQueryHandler<GetPropertyQuery, PropertyDto>
{
    public async Task<Result<PropertyDto>> Handle(...)
    {
        // Read logic
    }
}
```

## 📋 Migration Strategy

### Development

```bash
# Hər modul üçün ayrıca migration
dotnet ef migrations add InitialCreate \
    -p MTK.Modules.Properties \
    -s MTK.Api \
    -c PropertiesDbContext \
    -o Infrastructure/Migrations

dotnet ef database update \
    -p MTK.Modules.Properties \
    -s MTK.Api \
    -c PropertiesDbContext
```

### Production

```bash
# Script generation
dotnet ef migrations script \
    -p MTK.Modules.Properties \
    -s MTK.Api \
    -c PropertiesDbContext \
    -o migrations.sql
```

## ✅ Best Practices

### 1. Module Independence
```csharp
// ❌ WRONG: Direct module dependency
using MTK.Modules.Tenants.Domain;

// ✅ CORRECT: Use domain events
property.RaiseDomainEvent(new PropertyCreatedDomainEvent(propertyId));
```

### 2. Shared Code
```csharp
// ✅ CORRECT: Shared code in MTK.Common
using MTK.Common.ValueObjects;

var price = new Money(100, Currency.Azn);
```

### 3. Database Access
```csharp
// ❌ WRONG: Access other module's DbContext
using MTK.Modules.Tenants.Infrastructure;

// ✅ CORRECT: Use domain events or integration events
```

## 🔄 Migration Path

### From Layered to Modular

1. ✅ Create MTK.Common (done)
2. ✅ Create modules structure (done)
3. 🔄 Migrate entities to modules (in progress)
4. ⏳ Remove old layers (later)

### From Modular Monolith to Microservices

Hər modul müstəqil microservice-ə çevrilə bilər:

```
MTK.Modules.Properties → Properties.Service (Docker container)
MTK.Modules.Payments   → Payments.Service (Docker container)
```

Domain Events → Integration Events (RabbitMQ/Kafka)

## 📊 Current Structure

```
MTK/
├── src/
│   ├── Apps/
│   │   └── ✅ MTK.Api/                       # Host application
│   ├── Common/
│   │   ├── ✅ MTK.Common.Domain/             # Base abstractions
│   │   ├── ✅ MTK.Common.Application/        # CQRS, ValidationBehavior
│   │   ├── ✅ MTK.Common.Infrastructure/     # Infrastructure abstractions
│   │   └── ✅ MTK.Common.Presentation/       # Base controllers
│   └── Modules/
│       └── Properties/
│           ├── ✅ MTK.Modules.Properties.Domain/
│           ├── ✅ MTK.Modules.Properties.Application/
│           ├── ✅ MTK.Modules.Properties.Infrastructure/
│           ├── ✅ MTK.Modules.Properties.IntegrationEvents/
│           └── ✅ MTK.Modules.Properties.Presentation/
├── ✅ Directory.Packages.props               # Central package management
└── ✅ MTK.sln                                # Solution file
```

## 🎓 Next Steps

1. **Implement Tenants Module**
   - Create 5-layer structure like Properties
   - Tenant entity with domain events
   - Commands/Queries

2. **Implement Payments Module**
   - Create 5-layer structure
   - Subscribe to PropertyCreatedIntegrationEvent
   - Payment processing logic

3. **Implement Services Module**
   - Create 5-layer structure
   - Service types and assignments
   - Integration with Tenants

4. **Add Integration Events Infrastructure**
   - Outbox pattern for domain events
   - Inbox pattern for integration events
   - Background worker for event processing

5. **Add Integration Tests**
   - Test inter-module communication
   - Test domain and integration events

## ✅ Completed

- ✅ Fivestar-api strukturu
- ✅ Clean Architecture + DDD
- ✅ Modular Monolith pattern
- ✅ CQRS with MediatR
- ✅ FluentValidation pipeline
- ✅ Central package management
- ✅ Properties module (complete example)

Modular Monolith ready! 🚀
