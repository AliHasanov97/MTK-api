using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class GarageRepository : SearchableRepository<Garage>, IGarageRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public GarageRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<List<Garage>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);

        // Include navigation properties
        query = query.Include(g => g.Owner);

        // Apply search term
        string? searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Garage>> GetByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Garages
            .Include(g => g.Owner)
            .Where(g => g.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(
        string garageNumber,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Garages
            .AnyAsync(g => g.GarageNumber == garageNumber, cancellationToken);
    }

    public override async Task<Garage?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Garages
            .Include(g => g.Owner)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }
}
