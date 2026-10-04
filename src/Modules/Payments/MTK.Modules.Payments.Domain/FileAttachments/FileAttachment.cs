using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Contracts;
using MTK.Modules.Payments.Domain.Owners;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.Users;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Domain.FileAttachments;

/// <summary>
/// Müqaviləyə, tədarükçüyə, ödənişə, sahibə və ya tranzaksiyaya (xərc/əlavə gəlir)
/// aid yüklənmiş sənəd (imzalanmış müqavilə skanı, ödəniş qəbzi, xərc qaiməsi və s.).
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

    public Guid? ContractId { get; private set; }
    public Contract? Contract { get; private set; }

    public Guid? VendorId { get; private set; }
    public Vendor? Vendor { get; private set; }

    public Guid? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }

    public Guid? OwnerId { get; private set; }
    public Owner? Owner { get; private set; }

    public Guid? TransactionId { get; private set; }
    public Transaction? Transaction { get; private set; }

    // Only the person who uploaded a signed/scanned document may delete it again —
    // everyone else (including residents, who can always download it) is read-only.
    // Set server-side from the authenticated request, never client-supplied.
    public Guid? UploadedByUserId { get; private set; }
    public User? UploadedBy { get; private set; }

    public static FileAttachment Create(
        string fileName,
        string objectKey,
        string contentType,
        long sizeBytes,
        Guid? contractId,
        Guid? vendorId,
        Guid? paymentId,
        Guid? ownerId,
        Guid? transactionId,
        Guid? uploadedByUserId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Fayl adı mütləqdir", nameof(fileName));
        }

        if (!contractId.HasValue && !vendorId.HasValue && !paymentId.HasValue
            && !ownerId.HasValue && !transactionId.HasValue)
        {
            throw new ArgumentException(
                "Ən azı bir əlaqəli obyekt (müqavilə, tədarükçü, ödəniş, sahib və ya " +
                "tranzaksiya) göstərilməlidir.");
        }

        var attachment = new FileAttachment
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            ObjectKey = objectKey,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            ContractId = contractId,
            VendorId = vendorId,
            PaymentId = paymentId,
            OwnerId = ownerId,
            TransactionId = transactionId,
            UploadedByUserId = uploadedByUserId,
        };

        attachment.SetCreatedAt();

        return attachment;
    }
}
