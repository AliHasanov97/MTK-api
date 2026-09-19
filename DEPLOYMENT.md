# 🚀 MTK - Dokploy Deployment Guide

Bu təlimat MTK proyektini Dokploy-da Docker Compose ilə deploy etməyi izah edir.

## 📋 Tələblər

- Dokploy quraşdırılmış server
- Domain name (məsələn: `mtk.yourdomain.com`)
- SSL sertifikatı (Dokploy avtomatik Lets Encrypt istifadə edə bilər)

## 🎯 Deployment Addımları

### 1. Dokploy-da Yeni Application Yaradın

1. **Dokploy Dashboard** açın
2. **New Application** → **Docker Compose** seçin
3. **Repository URL**: `https://github.com/AliHasanov97/MTK-api.git`
4. **Branch**: `master`
5. **Compose File Path**: `docker-compose.yml`

### 2. Environment Variables Konfiqurasiyası

**Environment** tab-ına gedin və aşağıdakı dəyərləri əlavə edin:

#### PostgreSQL (Keycloak Database)
```env
KEYCLOAK_DB_NAME=keycloak
KEYCLOAK_DB_USER=keycloak
KEYCLOAK_DB_PASSWORD=YOUR_STRONG_PASSWORD_HERE
```

#### Keycloak Admin
```env
KEYCLOAK_ADMIN_USER=admin
KEYCLOAK_ADMIN_PASSWORD=YOUR_STRONG_PASSWORD_HERE
```

#### Keycloak Production Settings
```env
KC_COMMAND=start
KC_HOSTNAME=mtk.yourdomain.com
KC_HOSTNAME_STRICT=true
KC_HOSTNAME_STRICT_HTTPS=true
KC_HTTP_ENABLED=false
```

#### PostgreSQL (MTK Application Database)
```env
MTK_DB_NAME=MTK
MTK_DB_USER=mtkuser
MTK_DB_PASSWORD=YOUR_STRONG_PASSWORD_HERE
```

### 3. Deploy Edin

**Deploy** düyməsinə basın və deployment prosesini izləyin.

### 4. Keycloak Quraşdırması

Deployment tamamlandıqdan sonra:

1. Brauzerə gedin: `https://mtk.yourdomain.com/admin`
2. Admin Console-a daxil olun:
   - Username: `admin`
   - Password: `KEYCLOAK_ADMIN_PASSWORD` (environment variable-da təyin etdiyiniz)

3. [KEYCLOAK-SETUP.md](KEYCLOAK-SETUP.md) təlimatlarını izləyin:
   - `mtk` realm yaradın
   - `mtk-auth` və `mtk-admin` client-lər yaradın
   - Roles yaradın (admin, manager, user)
   - Client secret-ləri kopyalayın

### 5. MTK Application Environment Variables

Keycloak-dan client secret-ləri əldə etdikdən sonra, Dokploy-da əlavə environment variables əlavə edin:

```env
ConnectionStrings__Database=Host=mtk-postgres;Port=5432;Database=MTK;Username=mtkuser;Password=YOUR_MTK_DB_PASSWORD;Include Error Detail=false

Keycloak__Realm=mtk
Keycloak__OpenIdUrl=https://mtk.yourdomain.com/realms/mtk/protocol/openid-connect/
Keycloak__TokenUrl=https://mtk.yourdomain.com/realms/mtk/protocol/openid-connect/token
Keycloak__AuthClientId=mtk-auth
Keycloak__AuthClientSecret=YOUR_MTK_AUTH_CLIENT_SECRET
Keycloak__AdminClientId=mtk-admin
Keycloak__AdminClientSecret=YOUR_MTK_ADMIN_CLIENT_SECRET
Keycloak__AdminUrl=https://mtk.yourdomain.com/admin/realms/mtk/
Keycloak__AuthorizationUrl=https://mtk.yourdomain.com/realms/mtk/protocol/openid-connect/auth

Authentication__Audience=mtk-auth
Authentication__ValidIssuer=https://mtk.yourdomain.com/realms/mtk
Authentication__MetadataUrl=https://mtk.yourdomain.com/realms/mtk/.well-known/openid-configuration
Authentication__RequireHttpsMetadata=true

KeycloakSync__IntervalInMinutes=30
KeycloakSync__BatchSize=100
```

**Redeploy** edin ki, yeni environment variables tətbiq olunsun.

### 6. Database Migration

Application ilk dəfə işə düşdükdə, database migration avtomatik işləməyəcək. Migration etmək üçün:

**Option 1: Container-ə daxil olun**
```bash
# Dokploy-da container ID tapın
docker ps | grep mtk

# Container-ə daxil olun
docker exec -it <mtk-api-container-id> /bin/bash

# Migration işlədin
dotnet ef database update -p /app/src/Modules/Identity/MTK.Modules.Identity.Infrastructure -s /app/src/Apps/MTK.Api -c IdentityDbContext
```

**Option 2: Migration script əlavə edin**

Dockerfile-a startup script əlavə edə bilərsiniz ki, avtomatik migration olsun.

### 7. Domain və SSL

Dokploy-da **Domains** tab-ına gedin:

1. **Add Domain**: `mtk.yourdomain.com`
2. **Enable SSL** (Let's Encrypt)
3. **Force HTTPS**: Aktiv edin

### 8. Port Mapping (Keycloak üçün)

Keycloak container-i üçün port 8080-i expose etməlisiniz:

1. Dokploy-da **Ports** tab-ına gedin
2. Port **8080** → **8080** map edin
3. Keycloak-a access: `https://mtk.yourdomain.com:8080/admin`

və ya

**Reverse Proxy** konfiqurasiya edə bilərsiniz:
- Keycloak: `https://auth.mtk.yourdomain.com`
- MTK API: `https://api.mtk.yourdomain.com`

## ✅ Test

Deployment tamamlandıqdan sonra:

1. **Keycloak**: `https://mtk.yourdomain.com/admin`
2. **Swagger** (əgər aktivləşdirilibsə): `https://api.mtk.yourdomain.com/swagger`
3. **Health Check**: `https://api.mtk.yourdomain.com/health`

## 🔒 Təhlükəsizlik

✅ **Edilənlər:**
- SSL/HTTPS istifadə edilir
- Strong password-lar
- Keycloak production mode (`start`)
- HTTPS strict mode aktiv

⚠️ **Diqqət:**
- Default password-ları dəyişdirin
- `.env` faylını repository-ə push etməyin
- Production-da debug mode-u disable edin
- Regular backup-lar götürün (PostgreSQL volumes)

## 📊 Monitoring

Dokploy-da **Logs** tab-ından container log-larını izləyə bilərsiniz:

- **Keycloak logs**: `docker logs mtk-keycloak -f`
- **PostgreSQL logs**: `docker logs mtk-postgres -f`
- **MTK App logs**: `docker logs mtk-app-postgres -f`

## 🆘 Troubleshooting

### Problem: Keycloak iframe cookie error
**Həll**: Production-da domain istifadə etdikdə bu problem olmur. `KC_HOSTNAME` düzgün təyin edilməlidir.

### Problem: Database connection error
**Həll**: Environment variables-də password-ları yoxlayın. Container-lər arasında network əlaqəsi olduğundan əmin olun.

### Problem: Migration failure
**Həll**: Container-ə daxil olub manual migration edin və ya startup script əlavə edin.

---

**Suallarınız varsa:** [GitHub Issues](https://github.com/AliHasanov97/MTK-api/issues)
