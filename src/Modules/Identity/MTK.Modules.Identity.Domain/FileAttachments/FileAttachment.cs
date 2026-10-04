using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Domain.FileAttachments;

/// <summary>
/// İstifadəçiyə aid yüklənmiş sənəd (profil şəkli, şəxsiyyət vəsiqəsi skanı və s.).
/// Faylın özü Cloudflare R2-də saxlanılır — burada yalnız <see cref="ObjectKey"/>
/// (R2-dəki obyektin açarı) və metadata saxlanılır.
/// </summary>
public sealed class FileAttachment : Entity
{
    private FileAttachment() : base() { }

    public string FileName { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public static FileAttachment Create(
        string fileName,
        string objectKey,
        string contentType,
        long sizeBytes,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Fayl adı mütləqdir", nameof(fileName));
        }

        var attachment = new FileAttachment
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            ObjectKey = objectKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            UserId = userId,
        };

        attachment.SetCreatedAt();

        return attachment;
    }
}
