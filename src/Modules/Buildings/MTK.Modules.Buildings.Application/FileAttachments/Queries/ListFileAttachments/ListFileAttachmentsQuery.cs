using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.FileAttachments.Queries.ListFileAttachments;

/// <summary>Düz bir əlaqəli obyekt göstərilir — digərləri boş saxlanılır.</summary>
public sealed record ListFileAttachmentsQuery(
    Guid? BuildingId = null,
    Guid? ApartmentId = null,
    Guid? GarageId = null,
    Guid? OwnerId = null) : IQuery<List<FileAttachmentResponse>>;

public sealed record FileAttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAt);
