# MTK Data Seeder

Test məqsədilə sistemə avtomatik data əlavə edən console application.

## 📊 Yaradılan Data

- **3 bina** (Yasamal Residence A, B, C)
  - Hər binada 10 mərtəbə
  - Hər mərtəbədə 12 mənzil
  - **Cəmi: 360 mənzil**

- **~300 sakin (Owner)**
  - Azərbaycan adları ilə (Bogus faker)
  - Telefon nömrələri: +994##-###-##-##
  - Email: faker.email@example.com

- **30 qaraj**
  - G-001, G-002, ... G-030
  - Növlər: UndergroundParking, OpenParking, ClosedGarage

## 🏠 Ownership (Sakinlik) Qaydası

- ~80% mənzillərdə sakin var (~290 mənzil)
- ~70 mənzil boş
- ~20 sakin 2-3 mənzilə sahibdir
- Bütün qarajlar random sakinlərə təyin olunur

## ⚙️ İstifadə

### 1. Database hazır olmalıdır

```bash
# Migrations apply et
cd src/Apps/MTK.Api
dotnet ef database update
```

### 2. Seeder işə sal

```bash
cd src/Apps/MTK.DataSeeder
dotnet run
```

### 3. Prosesi izlə

```
🌱 MTK Data Seeder başladı...

📦 1/5 - Buildings seed edilir...
  ✅ Bina yaradıldı: Yasamal Residence A
  ✅ Bina yaradıldı: Yasamal Residence B
  ✅ Bina yaradıldı: Yasamal Residence C
  📊 Cəmi 3 bina yaradıldı
✅ Buildings seed tamamlandı!

👥 2/5 - Owners (Sakinlər) seed edilir...
  ✅ 50 sakin yaradıldı...
  ✅ 100 sakin yaradıldı...
  ...
  📊 Cəmi 300 sakin yaradıldı
✅ Owners seed tamamlandı!

🏠 3/5 - Apartments (Mənzillər) seed edilir...
  🏢 Bina 1 üçün mənzillər yaradılır...
  ✅ Bina 1 üçün 120 mənzil yaradıldı
  ...
  📊 Cəmi 360 mənzil yaradıldı
✅ Apartments seed tamamlandı!

🚗 4/5 - Garages (Qarajlar) seed edilir...
  📊 Cəmi 30 qaraj yaradıldı
✅ Garages seed tamamlandı!

🔗 5/5 - Ownerships (Sakinlik təyin edilir)...
  ✅ 290 mənzilə sakin təyin edildi
  ✅ Cəmi 310 mənzil-sakin əlaqəsi yaradıldı
  ✅ 30 qaraj-sakin əlaqəsi yaradıldı

  📊 Statistika:
    - Mənzillər: 360 cəmi, 310 təyin edilib, 50 boş
    - Qarajlar: 30 cəmi, 30 təyin edilib
✅ Ownerships seed tamamlandı!

🎉 Bütün seed prosesi uğurla tamamlandı!

  - 3 bina
  - 360 mənzil (120 x 3)
  - 30 qaraj
  - ~300 sakin

✅ Integration events avtomatik Payments module-a göndərildi!
✅ Cache invalidation işlədi!
```

## 🚀 Necə İşləyir

### MediatR Commands İstifadəsi

Seeder birbaşa database-ə **yazmır**. Əvəzinə MediatR commands istifadə edir:

```csharp
// Building yarat
var command = new CreateBuildingCommand(...);
var result = await _sender.Send(command);

// Owner yarat
var command = new CreateOwnerCommand(...);
await _sender.Send(command);

// Apartment yarat
var command = new CreateApartmentCommand(...);
await _sender.Send(command);
```

### ✅ Üstünlükləri

1. **Integration Events işləyir**
   - Building yaradılanda → `BuildingCreatedIntegrationEvent`
   - Payments module avtomatik sync olur

2. **Domain Events işləyir**
   - Business logic təmin olunur
   - Validation işləyir

3. **Cache Invalidation işləyir**
   - Yeni data yaradılanda cache avtomatik silinir
   - Payments module cache-i yenilənir

4. **Business Logic Bypass olunmur**
   - Bütün validation qaydalara riayət olunur
   - Domain constraints təmin edilir

## 🔧 Configuration

`appsettings.json` faylında database connection string:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=mtk_dev;Username=postgres;Password=postgres"
  }
}
```

## 📝 Qeydlər

- **User ID-lər:** Fake Guid-lər yaradılır (real sistemdə Identity module-dan gələcək)
- **Integration Events:** MassTransit/RabbitMQ işləyirsə, Payments module avtomatik sync olacaq
- **Cache:** Redis işləyirsə, cache avtomatik invalidate olacaq

## ⚠️ Diqqət

- Database-də mövcud data üzərinə yazır (duplicate yaratmır)
- İkinci dəfə işlətdikdə xəta verə bilər (unique constraint)
- Database-i təmizləmək üçün: `dotnet ef database drop -f` və yenidən `dotnet ef database update`
