# 🚀 MTK - Sürətli Başlanğıc

## 1️⃣ Docker Container-ləri İşə Salın

```bash
docker-compose up -d
```

✅ Bu əmr işə salır:
- Keycloak (http://localhost:8080)
- PostgreSQL (Port 5432 - Keycloak database)
- PostgreSQL (Port 5433 - MTK application database)

## 2️⃣ Keycloak-u Quraşdırın

### Keycloak Admin Console-a Giriş

1. Brauzerdə açın: **http://localhost:8080**
2. **Administration Console** → Login:
   - Username: `admin`
   - Password: `admin123`

### MTK Realm Yaradın

1. Sol üst künc → **Master** dropdown → **Create Realm**
2. Realm name: `mtk` → **Create**

### Client-lər Yaradın

#### Client 1: mtk-auth

1. **Clients** → **Create client**
2. Client ID: `mtk-auth` → **Next**
3. Client authentication: **ON** ✅
4. Standard flow: **ON** ✅
5. Implicit flow: **ON** ✅ (Swagger üçün)
6. Direct access grants: **ON** ✅
7. **Next**
8. Valid redirect URIs: `http://localhost:5000/*`
9. Web origins: `http://localhost:5000`
10. **Save**
11. **Credentials** tab → **Client secret-i kopyalayın** 📋

#### Client 2: mtk-admin

1. **Clients** → **Create client**
2. Client ID: `mtk-admin` → **Next**
3. Client authentication: **ON** ✅
4. Service accounts roles: **ON** ✅
5. **Save**
6. **Credentials** tab → **Client secret-i kopyalayın** 📋
7. **Service Account Roles** tab:
   - **Assign role** → Filter by clients
   - `realm-management` seçin:
     - ✅ manage-users
     - ✅ manage-clients
     - ✅ view-users
   - **Assign**

### Roles Yaradın

1. **Realm roles** → **Create role**
2. Yaradın:
   - Role name: `admin` → Save
   - Role name: `manager` → Save
   - Role name: `user` → Save

### Test İstifadəçi

1. **Users** → **Create new user**
2. Username: `testuser`
3. Email: `test@mtk.com`
4. First name: `Test`, Last name: `User`
5. Email verified: **ON**
6. **Create**
7. **Credentials** tab → Set password: `Test123!` (Temporary: OFF)
8. **Role mapping** → Assign: `user`

## 3️⃣ appsettings.json-u Yeniləyin

Fayl: `src/Apps/MTK.Api/appsettings.json`

```json
{
  "Keycloak": {
    "AuthClientSecret": "BURAYA_MTK_AUTH_SECRET",
    "AdminClientSecret": "BURAYA_MTK_ADMIN_SECRET"
  }
}
```

Kopyaladığınız client secret-ləri yapışdırın!

## 4️⃣ Database Migration

```bash
dotnet ef migrations add InitialIdentity -p src/Modules/Identity/MTK.Modules.Identity.Infrastructure -s src/Apps/MTK.Api -c IdentityDbContext -o Database/Migrations

dotnet ef database update -p src/Modules/Identity/MTK.Modules.Identity.Infrastructure -s src/Apps/MTK.Api -c IdentityDbContext
```

## 5️⃣ Application-u İşə Salın

```bash
cd src/Apps/MTK.Api
dotnet run
```

## 6️⃣ Swagger-də Test Edin

1. Açın: **http://localhost:5000/swagger**
2. **Authorize** düyməsinə basın
3. Keycloak login səhifəsi açılacaq
4. `testuser` / `Test123!` ilə login edin
5. API-ları test edin! 🎉

---

## ✅ Hazırdır!

İndi bu endpoint-ləri test edə bilərsiniz:

- `POST /api/users/register` - Yeni istifadəçi
- `GET /api/users/me` - Cari istifadəçi məlumatları

Ətraflı məlumat üçün: [KEYCLOAK-SETUP.md](KEYCLOAK-SETUP.md)
