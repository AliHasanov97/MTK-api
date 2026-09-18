# MTK - Mənzil Təsərrüfatı Kompleksi İdarəetmə Sistemi

Mənzil sakinləri və idarəetmə komitəsi üçün maliyyə şəffaflığını təmin edən modular monolith proqram.

## 🏗️ Arxitektura

- **Modular Monolith** pattern (fivestar-api strukturu)
- **Clean Architecture** + **DDD** (Domain-Driven Design)
- **CQRS** (Command Query Responsibility Segregation)
- **Vertical Slice Architecture**
- **Keycloak** authentication & authorization
- **.NET 9.0** + **Entity Framework Core**

## 📦 Modullar

- ✅ **Identity Module** - İstifadəçi idarəetməsi və Keycloak inteqrasiyası
- 🔜 **Properties Module** - Mənzil və bina idarəetməsi
- 🔜 **Tenants Module** - Sakin idarəetməsi
- 🔜 **Payments Module** - Ödəniş idarəetməsi
- 🔜 **Services Module** - Xidmət idarəetməsi

## 🚀 Quraşdırılma

**Sürətli başlanğıc üçün**: [START.md](START.md) ⚡

### 1. Tələblər

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 2. Docker Container-ləri İşə Salın

```bash
docker-compose up -d
```

Bu əmr aşağıdakı servislər işə salır:
- **Keycloak 25.0.6**: http://localhost:8080
- **PostgreSQL 16** (Keycloak): Port 5432 - Keycloak database
- **PostgreSQL 16** (MTK): Port 5433 - MTK application database

### 3. Keycloak Quraşdırın

Ətraflı təlimatlar: [KEYCLOAK-SETUP.md](KEYCLOAK-SETUP.md)

**Qısa yol**:
1. http://localhost:8080 → Admin Console
2. Login: `admin` / `admin123`
3. Realm yaradın: `mtk`
4. Client-lər yaradın: `mtk-auth` və `mtk-admin`
5. Roles və test istifadəçilər əlavə edin

### 4. appsettings.json Yeniləyin

```bash
cd src/Apps/MTK.Api
```

Keycloak client secret-lərini `appsettings.json`-a əlavə edin.

### 5. Database Migration

```bash
dotnet ef migrations add InitialIdentity \
  -p src/Modules/Identity/MTK.Modules.Identity.Infrastructure \
  -s src/Apps/MTK.Api \
  -c IdentityDbContext \
  -o Database/Migrations

dotnet ef database update \
  -p src/Modules/Identity/MTK.Modules.Identity.Infrastructure \
  -s src/Apps/MTK.Api \
  -c IdentityDbContext
```

### 6. Application-u İşə Salın

```bash
cd src/Apps/MTK.Api
dotnet run
```

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## 📖 API Documentation

### Authentication

#### İstifadəçi Qeydiyyatı
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "Password123!",
  "phoneNumber": "+994501234567"
}
```

#### Cari İstifadəçi
```http
GET /api/users/me
Authorization: Bearer {token}
```

## 🔑 Keycloak

| Resurs | URL |
|--------|-----|
| Admin Console | http://localhost:8080 |
| Realm | http://localhost:8080/realms/mtk |
| OpenID Config | http://localhost:8080/realms/mtk/.well-known/openid-configuration |

**Admin Credentials:**
- Username: `admin`
- Password: `admin123`

## 📁 Struktur

```
MTK/
├── src/
│   ├── Apps/
│   │   └── MTK.Api/                    # Web API
│   ├── Common/
│   │   ├── MTK.Common.Domain/          # Base abstractions
│   │   ├── MTK.Common.Application/     # CQRS, Authorization
│   │   ├── MTK.Common.Infrastructure/  # Infrastructure
│   │   └── MTK.Common.Presentation/    # Base controllers
│   └── Modules/
│       └── Identity/
│           ├── MTK.Modules.Identity.Domain/
│           ├── MTK.Modules.Identity.Application/
│           ├── MTK.Modules.Identity.Infrastructure/
│           ├── MTK.Modules.Identity.IntegrationEvents/
│           └── MTK.Modules.Identity.Presentation/
├── docker-compose.yml                   # Keycloak + Databases
├── Directory.Packages.props
└── MTK.sln
```

## 🛠️ Texnologiyalar

- **.NET 9.0** - Runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9.0** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Validation
- **Keycloak** - Identity & Access Management
- **PostgreSQL** - Database
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization

## 📚 Sənədlər

- [START.md](START.md) - Sürətli başlanğıc
- [KEYCLOAK-SETUP.md](KEYCLOAK-SETUP.md) - Keycloak quraşdırılması
- [MODULAR-MONOLITH.md](MODULAR-MONOLITH.md) - Arxitektura
- [STRUCTURE.md](STRUCTURE.md) - Proyekt strukturu

## 🧪 Test

```bash
# Build
dotnet build

# Run
cd src/Apps/MTK.Api
dotnet run

# Swagger
http://localhost:5000/swagger
```

## 🤝 Development

```bash
# Git clone
git clone <repository-url>
cd MTK

# Docker-ləri işə salın
docker-compose up -d

# Packages restore
dotnet restore

# Build
dotnet build

# Database migration
dotnet ef database update -p src/Modules/Identity/MTK.Modules.Identity.Infrastructure -s src/Apps/MTK.Api -c IdentityDbContext

# Run
cd src/Apps/MTK.Api
dotnet run
```

## ✅ Xüsusiyyətlər

- ✅ Fivestar-api strukturu
- ✅ Clean Architecture + DDD
- ✅ CQRS Pattern (MediatR)
- ✅ Keycloak Authentication
- ✅ JWT Bearer Authentication
- ✅ Role-based Authorization
- ✅ FluentValidation
- ✅ Central Package Management
- ✅ Identity Module (User management)
- ✅ Docker Compose (Keycloak + Databases)
- ✅ Swagger OAuth2
- ⏳ Integration Events
- ⏳ Outbox/Inbox Pattern
- ⏳ Digər modullar

## 🐳 Docker Əmrləri

```bash
# Start containers
docker-compose up -d

# Stop containers
docker-compose down

# View logs
docker-compose logs -f keycloak

# Restart
docker-compose restart

# Clean restart (deletes data)
docker-compose down -v
docker-compose up -d
```

## 📝 License

MIT License

---

**Hazırlanıb** Vahid üçün ❤️
