using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.Employees;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class EmployeeRepository : SearchableRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(HrDbContext context) : base(context) { }

    public override async Task<Employee?> GetByIdDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Employee>()
            .Include(e => e.Job)
            .Include(e => e.CreatedBy)
            .Include(e => e.WorkHistories)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public override async Task<List<Employee>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> query = ApplyFiltersAndSort(filters, sortCriteria)
            .Include(e => e.Job);

        // Ad / soyad / ata adı üzrə tam mətn axtarışı (CountAsync əsas sinifdə eyni şərti tətbiq edir)
        var searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query.Where(e => e.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }

        return await ApplyPages(query, page, pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetNextRegisterNumberAsync(CancellationToken cancellationToken = default)
    {
        // Silinmiş işçilərin sıra nömrəsi təkrar verilməsin deyə query filter-siz hesablanır
        var maxRegisterNumber = await Context.Set<Employee>()
            .IgnoreQueryFilters()
            .MaxAsync(e => (int?)e.RegisterNumber, cancellationToken);

        return (maxRegisterNumber ?? 0) + 1;
    }

    public async Task<Employee?> GetByIdWithWorkHistoriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Employee>()
            .Include(e => e.Job)
            .Include(e => e.CreatedBy)
            .Include(e => e.WorkHistories)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Employee>> GetForTimesheetAsync(
        DateOnly monthEnd,
        CancellationToken cancellationToken = default)
    {
        var monthEndDateTime = monthEnd.ToDateTime(TimeOnly.MaxValue);

        return await Context.Set<Employee>()
            .Include(e => e.Job)
            .Include(e => e.WorkSchedules)
            .Where(e => e.StartWorkDate <= monthEndDateTime)
            .OrderBy(e => e.Surname)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Employee>()
            .Include(e => e.Job)
            .OrderBy(e => e.Surname)
            .ThenBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }
}
