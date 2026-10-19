using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Buildings.Domain.Appeals;
using MTK.Modules.Buildings.Infrastructure.Database;

namespace MTK.Modules.Buildings.Infrastructure.Repositories;

internal sealed class AppealRepository : Repository<Appeal>, IAppealRepository
{
    private BuildingsDbContext BuildingsContext => (BuildingsDbContext)Context;

    public AppealRepository(BuildingsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Appeal?> GetWithCommentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Appeals
            .Include(a => a.Comments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Appeal>> GetBySubmitterIdAsync(Guid submitterId, CancellationToken cancellationToken = default)
    {
        return await BuildingsContext.Appeals
            .Where(a => a.SubmitterId == submitterId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetCommentCountsAsync(
        IReadOnlyCollection<Guid> appealIds,
        CancellationToken cancellationToken = default)
    {
        if (appealIds.Count == 0)
            return new Dictionary<Guid, int>();

        var ids = appealIds.Distinct().ToList();

        // Kommentlər soft-delete filteri ilə gəlir, ona görə silinmişlər sayılmır.
        return await BuildingsContext.AppealComments
            .Where(c => ids.Contains(c.AppealId))
            .GroupBy(c => c.AppealId)
            .Select(g => new { AppealId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AppealId, x => x.Count, cancellationToken);
    }
}
