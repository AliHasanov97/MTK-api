using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Buildings;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Owners;

namespace MTK.Modules.Buildings.Domain.FileAttachments;

/// <summary>
/// Binaya, mənzilə, qaraja və ya sahibə aid yüklənmiş sənəd (mülkiyyət sənədi,
/// razılaşma, şəxsiyyət vəsiqəsi skanı və s.). Faylın özü Cloudflare R2-də saxlanılır —
/// burada yalnız <see cref="ObjectKey"/> (R2-dəki obyektin açarı) və metadata saxlanılır.
/// </summary>
public sealed class FileAttachment : Entity
{
    private FileAttachment() : base() { }

    public string FileName { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }

    public Guid? BuildingId { get; private set; }
    public Building? Building { get; private set; }

    public Guid? ApartmentId { get; private set; }
    public Apartment? Apartment { get; private set; }

    public Guid? GarageId { get; private set; }
    public Garage? Garage { get; private set; }

    public Guid? OwnerId { get; private set; }
    public Owner? Owner { get; private set; }

    public static FileAttachment Create(
        string fileName,
        string objectKey,
        string contentType,
        long sizeBytes,
        Guid? buildingId,
        Guid? apartmentId,
        Guid? garageId,
        Guid? ownerId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Fayl adı mütləqdir", nameof(fileName));
        }

        if (!buildingId.HasValue && !apartmentId.HasValue && !garageId.HasValue && !ownerId.HasValue)
        {
            throw new ArgumentException(
                "Ən azı bir əlaqəli obyekt (bina, mənzil, qaraj və ya sahib) göstərilməlidir.");
        }

        var attachment = new FileAttachment
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            ObjectKey = objectKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            BuildingId = buildingId,
            ApartmentId = apartmentId,
            GarageId = garageId,
            OwnerId = ownerId,
        };

        attachment.SetCreatedAt();

        return attachment;
    }
}
