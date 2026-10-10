using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.JobApplications;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class JobApplicationRepository : SearchableRepository<JobApplication>, IJobApplicationRepository
{
    public JobApplicationRepository(HrDbContext context) : base(context)
    {
    }

    public override async Task<JobApplication?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<JobApplication>()
            .Include(j => j.CreatedBy)
            .Include(j => j.Job)
            .Include(j => j.EmploymentOrder)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public override async Task<List<JobApplication>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<JobApplication> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(j => j.CreatedBy)
            .Include(j => j.Job)
            .Include(j => j.EmploymentOrder);

        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }


}
