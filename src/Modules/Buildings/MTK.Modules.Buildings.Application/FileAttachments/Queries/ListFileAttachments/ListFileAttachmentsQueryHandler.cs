using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.FileAttachments;
using MTK.Modules.Buildings.Domain.Repositories;

namespace MTK.Modules.Buildings.Application.FileAttachments.Queries.ListFileAttachments;

internal sealed class ListFileAttachmentsQueryHandler : IQueryHandler<ListFileAttachmentsQuery, List<FileAttachmentResponse>>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;

    public ListFileAttachmentsQueryHandler(IFileAttachmentRepository fileAttachmentRepository)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
    }

    public async Task<Result<List<FileAttachmentResponse>>> Handle(
        ListFileAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        List<FileAttachment> attachments = request switch
        {
            { BuildingId: { } buildingId } => await _fileAttachmentRepository.ListByBuildingIdAsync(buildingId, cancellationToken),
            { ApartmentId: { } apartmentId } => await _fileAttachmentRepository.ListByApartmentIdAsync(apartmentId, cancellationToken),
            { GarageId: { } garageId } => await _fileAttachmentRepository.ListByGarageIdAsync(garageId, cancellationToken),
            { OwnerId: { } ownerId } => await _fileAttachmentRepository.ListByOwnerIdAsync(ownerId, cancellationToken),
            _ => [],
        };

        return attachments
            .Select(a => new FileAttachmentResponse(a.Id, a.FileName, a.ContentType, a.SizeBytes, a.CreatedAt))
            .ToList();
    }
}
