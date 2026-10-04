using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.FileAttachments;

namespace MTK.Modules.Buildings.Domain.Repositories;

public interface IFileAttachmentRepository : IRepository<FileAttachment>
{
    Task<List<FileAttachment>> ListByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByApartmentIdAsync(Guid apartmentId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByGarageIdAsync(Guid garageId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
