using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.Repositories;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class OwnerRepository : SearchableRepository<Owner>, IOwnerRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public OwnerRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    // tsvector search only does whole-token prefix matching, so a phone
    // number typed from the middle (e.g. "501234" out of "+994501234567")
    // would never match even once SearchVector is populated — PhoneNumber
    // gets an extra plain substring check here for that reason.
    public override async Task<List<Owner>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplySearchTerm(query, searchTerm);
        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<int> CountAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);
        query = ApplySearchTerm(query, searchTerm);
        return await query.CountAsync(cancellationToken);
    }

    private IQueryable<Owner> ApplySearchTerm(IQueryable<Owner> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        string? searchTermQuery = GetSearchTerm(searchTerm);
        var trimmedTerm = searchTerm.Trim();

        return query.Where(o =>
            (searchTermQuery != null && o.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)))
            || o.PhoneNumber.Contains(trimmedTerm));
    }

    public async Task<Owner?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<Owner?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Owner>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
            .Include(o => o.OwnedGarages)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public override async Task<Owner?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Owners
            .Include(o => o.OwnedApartments)
                .ThenInclude(a => a.Building)
            .Include(o => o.OwnedGarages)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
