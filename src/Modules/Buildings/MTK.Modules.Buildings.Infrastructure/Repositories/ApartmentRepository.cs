using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class ApartmentRepository : SearchableRepository<Apartment>, IApartmentRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public ApartmentRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<List<Apartment>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);

        // Include navigation properties
        query = query
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner);

        // Apply search term
        string? searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Apartment>> GetByBuildingIdAsync(
        Guid buildingId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Where(a => a.BuildingId == buildingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Apartment>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .Where(a => a.CurrentOwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Apartment>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(
        Guid buildingId,
        string apartmentNumber,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Apartments
            .AnyAsync(
                a => a.BuildingId == buildingId && a.ApartmentNumber == apartmentNumber,
                cancellationToken);
    }

    public override async Task<Apartment?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Apartments
            .Include(a => a.Building)
            .Include(a => a.CurrentOwner)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
