using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.FileAttachments;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class FileAttachmentRepository : Repository<FileAttachment>, IFileAttachmentRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public FileAttachmentRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<FileAttachment>> ListByBuildingIdAsync(Guid buildingId, CancellationToken cancellationToken = default)
    {
        return BuildingsContext.FileAttachments
            .Where(f => f.BuildingId == buildingId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByApartmentIdAsync(Guid apartmentId, CancellationToken cancellationToken = default)
    {
        return BuildingsContext.FileAttachments
            .Where(f => f.ApartmentId == apartmentId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByGarageIdAsync(Guid garageId, CancellationToken cancellationToken = default)
    {
        return BuildingsContext.FileAttachments
            .Where(f => f.GarageId == garageId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return BuildingsContext.FileAttachments
            .Where(f => f.OwnerId == ownerId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
