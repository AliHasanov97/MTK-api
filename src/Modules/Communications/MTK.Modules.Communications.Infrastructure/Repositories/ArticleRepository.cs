using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Communications.Domain.Articles;
using MTK.Modules.Communications.Infrastructure.Database;

namespace MTK.Modules.Communications.Infrastructure.Repositories;

internal sealed class ArticleRepository : SearchableRepository<Article>, IArticleRepository
{
    public ArticleRepository(CommunicationsDbContext context) : base(context)
    {
    }

    public override async Task<Article?> GetByIdDefaultAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Article>()
            .Include(a => a.CreatedBy)
            .Include(a => a.FileAttachments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Article>> GetLatestPublishedAsync(
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Article>()
            .Include(a => a.CreatedBy)
            .Include(a => a.FileAttachments)
            .Where(a => a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Article> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        ArticleStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Article>()
            .Include(a => a.CreatedBy)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Article?> GetByIdAndIncrementViewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var article = await Context.Set<Article>()
            .Include(a => a.CreatedBy)
            .Include(a => a.FileAttachments)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article is not null)
        {
            article.IncrementViewCount();
            await Context.SaveChangesAsync(cancellationToken);
        }

        return article;
    }

    public override async Task<List<Article>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Article> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(a => a.CreatedBy);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(a => a.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
}
