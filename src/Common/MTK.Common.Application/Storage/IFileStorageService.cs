namespace MTK.Common.Application.Storage;

/// <summary>
/// Object storage üçün paylaşılan abstraksiya — bütün modullar (Buildings, Identity,
/// Payments) eyni Cloudflare R2 bucket-indən bu interfeys üzərindən istifadə edir.
/// </summary>
public interface IFileStorageService
{
    /// <returns>Yadda saxlanılan obyektin açarı (ObjectKey) — FileAttachment-də saxlanılır.</returns>
    Task<string> UploadAsync(
        Stream fileStream,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default);

    /// <summary>Müvəqqəti (presigned) birbaşa yükləmə linki — faylı backend üzərindən axıtmadan.</summary>
    string GetPresignedDownloadUrl(string objectKey, TimeSpan validFor);
}
