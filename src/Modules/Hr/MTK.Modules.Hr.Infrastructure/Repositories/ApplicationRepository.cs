using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = MTK.Modules.Hr.Domain.Applications.Application;
using IApplicationRepository = MTK.Modules.Hr.Domain.Applications.IApplicationRepository;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class ApplicationRepository : Repository<ApplicationEntity>, IApplicationRepository
{
    public ApplicationRepository(HrDbContext context) : base(context) { }

    public override async Task<List<ApplicationEntity>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = ApplyFiltersAndSort(filters, sortCriteria);
        var pagedQuery = ApplyPages(baseQuery, page, pageSize);

        // Load all applications with common properties
        var query = pagedQuery
            .Include(a => a.CreatedBy);

        var applications = await query.ToListAsync(cancellationToken);
        var applicationIds = applications.Select(a => a.Id).ToList();

        // Batch load navigation properties for ApplicationForChangeOfPosition
        await Context.Set<ApplicationForChangeOfPosition>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.OrderForChangeOfPosition)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for JobApplication
        // JobApplication.RelatedJobApplicant returns itself, so we just need to ensure it's loaded
        await Context.Set<Domain.JobApplications.JobApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.EmploymentOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for VacationCompensationApplication
        await Context.Set<VacationCompensationApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.CompensationOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for UnpaidLeaveApplication
        await Context.Set<UnpaidLeaveApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.UnpaidLeaveOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for VacationApplication
        await Context.Set<VacationApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.VacationOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for EducationLeaveApplication
        await Context.Set<EducationLeaveApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.EducationLeaveOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for EmploymentStatusChangeApplication
        await Context.Set<EmploymentStatusChangeApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.EmploymentStatusChangeOrder)
            .LoadAsync(cancellationToken);

        // Batch load navigation properties for VacationReturnApplication
        await Context.Set<VacationReturnApplication>()
            .Where(a => applicationIds.Contains(a.Id))
            .Include(a => a.Employee)
            .Include(a => a.VacationReturnOrder)
            .LoadAsync(cancellationToken);

        return applications;
    }
}
