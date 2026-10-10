using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class NoticeOfChangeInWorkingConditionsRepository :
    SearchableRepository<NoticeOfChangeInWorkingConditions>,
    INoticeOfChangeInWorkingConditionsRepository
{
    public NoticeOfChangeInWorkingConditionsRepository(HrDbContext context)
        : base(context)
    {
    }

    public override async Task<NoticeOfChangeInWorkingConditions?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<NoticeOfChangeInWorkingConditions>()
            .Include(n => n.Employee)
                .ThenInclude(e => e.Job)
            .Include(n => n.CreatedBy)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public override async Task<List<NoticeOfChangeInWorkingConditions>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<NoticeOfChangeInWorkingConditions> query =
            ApplyFiltersAndSort(filters, sortCriteria)
                .Include(n => n.Employee)
                .Include(n => n.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}