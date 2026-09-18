# Keycloak Quraşdırılması - MTK Proyekti

## 🐳 Docker ilə Keycloak İşə Salın

### 1. Docker Container-ləri İşə Salın

```bash
docker-compose up -d
```

Bu əmr 3 container işə salır:
- **Keycloak**: http://localhost:8080
- **PostgreSQL**: Port 5432 (Keycloak database)
- **SQL Server**: Port 1433 (MTK application database)

### 2. Container Statusunu Yoxlayın

```bash
docker-compose ps
```

Keycloak hazır olana qədər 30-60 saniyə gözləyin.

---

## 🔑 Keycloak Admin Console-a Giriş

1. Brauzerdə açın: http://localhost:8080
2. **Administration Console** düyməsinə basın
3. Login edin:
   - **Username**: `admin`
   - **Password**: `admin123`

---

## ⚙️ MTK Realm Yaratmaq

### Addım 1: Yeni Realm Yaradın

1. Sol üst küncdə **Master** dropdown-a basın
2. **Create Realm** düyməsinə basın
3. **Realm name**: `mtk` yazın
4. **Create** düyməsinə basın

### Addım 2: Client Yaradın (mtk-auth)

1. Sol menüdən **Clients** seçin
2. **Create client** düyməsinə basın
3. **General Settings**:
   - Client type: `OpenID Connect`
   - Client ID: `mtk-auth`
   - Name: `MTK Authentication Client`
   - **Next**

4. **Capability config**:
   - ✅ Client authentication: `ON`
   - ✅ Authorization: `OFF`
   - ✅ Standard flow: `ON` (Authorization Code)
   - ✅ Direct access grants: `ON` (Direct Grant/Password)
   - ✅ Implicit flow: `ON` (Swagger üçün)
   - **Next**

5. **Login settings**:
   - Root URL: `http://localhost:5000`
   - Home URL: `http://localhost:5000`
   - Valid redirect URIs:
     - `http://localhost:5000/*`
     - `https://localhost:5001/*`
   - Valid post logout redirect URIs: `http://localhost:5000/*`
   - Web origins: `http://localhost:5000`
   - **Save**

6. **Credentials** tab-a keçin:
   - Client secret-i kopyalayın və saxlayın

### Addım 3: Admin Client Yaradın (mtk-admin)

1. **Clients** → **Create client**
2. **General Settings**:
   - Client ID: `mtk-admin`
   - Name: `MTK Admin Client`
   - **Next**

3. **Capability config**:
   - ✅ Client authentication: `ON`
   - ✅ Service accounts roles: `ON` (Client Credentials üçün)
   - ❌ Standard flow: `OFF`
   - ❌ Direct access grants: `OFF`
   - **Next**

4. **Login settings**:
   - **Save**

5. **Credentials** tab:
   - Client secret-i kopyalayın və saxlayın

6. **Service Account Roles** tab:
   - **Assign role** → **Filter by clients**
   - `realm-management` → Aşağıdakıları seçin:
     - ✅ `manage-users`
     - ✅ `manage-clients`
     - ✅ `manage-realm`
     - ✅ `view-users`
     - ✅ `view-clients`
   - **Assign**

### Addım 4: Realm Roles Yaradın

1. Sol menüdən **Realm roles** seçin
2. **Create role** düyməsinə basın
3. Aşağıdakı role-ları yaradın:

**Admin Role**:
- Role name: `admin`
- Description: `System Administrator`
- **Save**

**Manager Role**:
- Role name: `manager`
- Description: `Property Manager`
- **Save**

**User Role**:
- Role name: `user`
- Description: `Regular User`
- **Save**

### Addım 5: Test İstifadəçi Yaradın

1. Sol menüdən **Users** seçin
2. **Create new user** düyməsinə basın
3. **Create user**:
   - Username: `testuser`
   - Email: `test@mtk.com`
   - First name: `Test`
   - Last name: `User`
   - Email verified: `ON`
   - **Create**

4. **Credentials** tab-a keçin:
   - **Set password**:
     - Password: `Test123!`
     - Password confirmation: `Test123!`
     - Temporary: `OFF`
   - **Save**

5. **Role mapping** tab-a keçin:
   - **Assign role**
   - `user` role-u seçin
   - **Assign**

### Addım 6: Admin İstifadəçi Yaradın

1. **Users** → **Create new user**
2. Username: `admin`
3. Email: `admin@mtk.com`
4. First name: `Admin`, Last name: `User`
5. Email verified: `ON`
6. **Create**

7. **Credentials** → Set password: `Admin123!`
8. **Role mapping** → Assign role: `admin`

---

## 📝 appsettings.json Yeniləyin

Client secret-ləri əldə etdikdən sonra:

```json
{
  "Keycloak": {
    "Realm": "mtk",
    "OpenIdUrl": "http://localhost:8080/realms/mtk/protocol/openid-connect/",
    "TokenUrl": "http://localhost:8080/realms/mtk/protocol/openid-connect/token",
    "AuthClientId": "mtk-auth",
    "AuthClientSecret": "YOUR_MTK_AUTH_CLIENT_SECRET",
    "AdminClientId": "mtk-admin",
    "AdminClientSecret": "YOUR_MTK_ADMIN_CLIENT_SECRET",
    "AdminUrl": "http://localhost:8080/admin/realms/mtk/",
    "AuthorizationUrl": "http://localhost:8080/realms/mtk/protocol/openid-connect/auth"
  },
  "Authentication": {
    "Audience": "mtk-auth",
    "ValidIssuer": "http://localhost:8080/realms/mtk",
    "MetadataUrl": "http://localhost:8080/realms/mtk/.well-known/openid-configuration",
    "RequireHttpsMetadata": false
  },
  "ConnectionStrings": {
    "Database": "Server=localhost,1433;Database=MTK;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
  }
}
```

---

## 🧪 Test Edin

### 1. Database Migration

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

### 2. Application-u İşə Salın

```bash
cd src/Apps/MTK.Api
dotnet run
```

### 3. Swagger-i Açın

http://localhost:5000/swagger

**Authorize** düyməsinə basın:
- Client ID avtomatik doldurulacaq
- Keycloak login səhifəsi açılacaq
- `testuser` / `Test123!` ilə login edin
- Token alındıqdan sonra API-ları test edin

### 4. Test İstifadəçi Qeydiyyatı

```bash
curl -X POST http://localhost:5000/api/users/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@mtk.com",
    "firstName": "New",
    "lastName": "User",
    "password": "NewUser123!",
    "phoneNumber": "+994501234567"
  }'
```

### 5. Cari İstifadəçi Məlumatlarını Alın

Swagger-də login olduqdan sonra:
```
GET /api/users/me
```

---

## 🛠️ Docker Əmrləri

### Container-ləri Dayandırın
```bash
docker-compose down
```

### Container-ləri Yenidən Başladın
```bash
docker-compose restart
```

### Keycloak Log-larına Baxın
```bash
docker-compose logs -f keycloak
```

### Database Data-nı Silin (Təmiz Başlanğıc)
```bash
docker-compose down -v
docker-compose up -d
```

---

## 📊 Keycloak URL-ləri

| Resurs | URL |
|--------|-----|
| Admin Console | http://localhost:8080 |
| Realm: mtk | http://localhost:8080/realms/mtk |
| OpenID Config | http://localhost:8080/realms/mtk/.well-known/openid-configuration |
| Token Endpoint | http://localhost:8080/realms/mtk/protocol/openid-connect/token |
| Auth Endpoint | http://localhost:8080/realms/mtk/protocol/openid-connect/auth |

---

## ✅ Quraşdırma Tamamlandı!

Artıq Keycloak hazırdır və MTK API-si ilə inteqrasiya edilib. Swagger-dən istifadəçiləri qeydiyyatdan keçirə və authenticate edə bilərsiniz.
