# 🐳 Docker Compose Setup - MTK Backend

Bu sənəd MTK backend sisteminin Docker ilə qaldırılması üçün təlimatdır.

## 📦 Servislər

Docker Compose aşağıdakı servislər qaldırır:

| Servis | Port | Container Adı | İzahat |
|--------|------|---------------|---------|
| **PostgreSQL** | 5433 | mtk-postgres | Əsas verilənlər bazası |
| **Redis** | 6379 | mtk-redis | Cache servisi |
| **RabbitMQ** | 5672, 15672 | mtk-rabbitmq | Message broker |
| **Keycloak** | 8080 | mtk-keycloak | Authentication servisi |
| **Keycloak DB** | 5434 | mtk-keycloak-postgres | Keycloak-un verilənlər bazası |

---

## 🚀 Başlanğıc

### 1️⃣ Docker Desktop Quraşdır

Əgər yoxdursa:
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) yüklə
- Quraşdır və restart et
- Docker Desktop-ı aç

### 2️⃣ Servislər Qaldır

```bash
# MTK proyektinin root qovluğunda
cd "C:\Users\aliha\OneDrive\Рабочий стол\Vahid\backends\MTK"

# Bütün servislər qaldır
docker-compose up -d
```

**İzahat:**
- `up` = qaldır
- `-d` = detached mode (background-da)

### 3️⃣ Statusu Yoxla

```bash
# Bütün container-lər
docker-compose ps

# Log-lara bax
docker-compose logs -f

# Konkret servis log-u
docker-compose logs -f redis
```

---

## 🔍 Servis Yoxlamaları

### PostgreSQL

```bash
# Container-ə gir
docker exec -it mtk-postgres psql -U mtkuser -d MTK

# SQL sorğusu
SELECT version();

# Çıx
\q
```

### Redis

```bash
# Redis CLI
docker exec -it mtk-redis redis-cli

# Test et
PING
# PONG

# Cache-ləri yoxla
KEYS *

# Konkret key
GET "rate:apartment"

# TTL yoxla
TTL "rate:apartment"

# Çıx
EXIT
```

**Redis Monitor (real-time):**
```bash
# Bütün əməliyyatları monitor et
docker exec -it mtk-redis redis-cli MONITOR
```

### RabbitMQ

Management UI: http://localhost:15672
- Username: `mtk-admin`
- Password: `MTK@RabbitMQ123`

### Keycloak

Admin Console: http://localhost:8080
- Username: `admin`
- Password: `admin`

---

## 🛠️ İdarəetmə Əmrləri

### Başlat/Dayandır

```bash
# Hamısını başlat
docker-compose start

# Hamısını dayandır
docker-compose stop

# Restart
docker-compose restart

# Konkret servis
docker-compose restart redis
```

### Logs

```bash
# Hamısının log-u
docker-compose logs -f

# Son 100 sətir
docker-compose logs --tail=100

# Redis log
docker-compose logs -f redis
```

### Təmizləmə

```bash
# Container-ləri dayandır və sil
docker-compose down

# Volume-ləri də sil (DATA İTİRİLƏCƏK!)
docker-compose down -v

# Yenidən qaldır
docker-compose up -d
```

---

## 🎯 MTK API İstifadəsi

### API Qaldır

```bash
# .NET API
dotnet run --project src/Apps/MTK.Api

# və ya
cd src/Apps/MTK.Api
dotnet run
```

API avtomatik localhost servislərə qoşulacaq:
- PostgreSQL: `localhost:5433`
- Redis: `localhost:6379`
- RabbitMQ: `localhost:5672`
- Keycloak: `localhost:8080`

---

## 📊 Redis Cache İzləmə

### Cache Performance Yoxla

```bash
# Redis stats
docker exec -it mtk-redis redis-cli INFO stats

# Memory istifadəsi
docker exec -it mtk-redis redis-cli INFO memory

# Hit/Miss ratio
docker exec -it mtk-redis redis-cli INFO stats | grep keyspace
```

### Cache Təmizlə

```bash
# Konkret key sil
docker exec -it mtk-redis redis-cli DEL "rate:apartment"

# Pattern ilə sil
docker exec -it mtk-redis redis-cli --scan --pattern "rate:*" | xargs docker exec -it mtk-redis redis-cli DEL

# HAMISI (DİQQƏTLİ!)
docker exec -it mtk-redis redis-cli FLUSHALL
```

---

## 🔧 Redis Konfiqurasiya

Docker Compose-dakı Redis parametrləri:

| Parametr | Dəyər | İzahat |
|-----------|-------|---------|
| `maxmemory` | 512mb | Maksimum yaddaş |
| `maxmemory-policy` | allkeys-lru | Ən köhnə key-ləri avtomatik sil |
| `save` | 60 1000 | Hər 60 saniyədə 1000 dəyişiklik olarsa disk-ə yaz |
| `appendonly` | yes | Persistence aktivdir |

**512mb** = 300-500 mənzil + cache məlumatları üçün kifayətdir.

---

## 🐛 Troubleshooting

### Port artıq istifadədədir

```bash
# Port-u işğal edən prosesi tap
netstat -ano | findstr :6379

# Prosesi dayandır
taskkill /PID <PID> /F
```

### Redis qoşulmur

```bash
# Container işləyirmi?
docker ps | grep redis

# Log-da xəta varmı?
docker-compose logs redis

# Restart et
docker-compose restart redis
```

### Cache işləmir

```bash
# API log-larına bax
dotnet run --project src/Apps/MTK.Api

# Redis-ə əlaqə yoxla
docker exec -it mtk-redis redis-cli PING
```

---

## 📈 Performans Məsləhətləri

### 300+ Mənzil Üçün

1. **Redis maxmemory artır:**
   ```yaml
   command: >
     redis-server
     --maxmemory 1gb  # 512mb → 1gb
   ```

2. **Cache TTL optimallaşdır:**
   - Rates: 1 saat (tez-tez dəyişmir)
   - PropertyOwnership: 30 dəqiqə
   - Charges: 5 dəqiqə (tez-tez dəyişir)

3. **Monitor et:**
   ```bash
   # Real-time monitoring
   docker stats mtk-redis
   ```

---

## 🎉 Hazır!

Bütün servislər işləyir. İndi MTK API-ni qaldırın və cache performansını izləyin!

**Cache hit nümunəsi:**
```
[Debug] Apartment rate loaded from cache: 0.50 AZN/m²
```

**Uğurlar!** 🚀
