using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VacationPays;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class VacationPayRepository : Repository<VacationPay>, IVacationPayRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public VacationPayRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<VacationPay?> GetByVacationOrderIdAsync(
        Guid vacationOrderId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VacationPays
            .FirstOrDefaultAsync(vp => vp.VacationOrderId == vacationOrderId, cancellationToken);
    }

    public async Task<List<VacationPay>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.VacationPays
            .Where(vp => vp.EmployeeId == employeeId)
            .OrderByDescending(vp => vp.VacationStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacationPay>> GetUpcomingAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.VacationPays
            .Where(vp => vp.Status == VacationPayStatus.Pending &&
                         vp.DueDate >= fromDate &&
                         vp.DueDate <= toDate)
            .OrderBy(vp => vp.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacationPay>> GetAllAsync(
        VacationPayStatus? statusFilter,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? employeeNameFilter,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = PaymentsContext.VacationPays.AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(vp => vp.Status == statusFilter.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(vp => vp.DueDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(vp => vp.DueDate <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(employeeNameFilter))
        {
            query = query.Where(vp => vp.EmployeeFullName.Contains(employeeNameFilter));
        }

        return await query
            .OrderBy(vp => vp.DueDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        VacationPayStatus? statusFilter,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? employeeNameFilter,
        CancellationToken cancellationToken = default)
    {
        var query = PaymentsContext.VacationPays.AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(vp => vp.Status == statusFilter.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(vp => vp.DueDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(vp => vp.DueDate <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(employeeNameFilter))
        {
            query = query.Where(vp => vp.EmployeeFullName.Contains(employeeNameFilter));
        }

        return await query.CountAsync(cancellationToken);
    }
}
