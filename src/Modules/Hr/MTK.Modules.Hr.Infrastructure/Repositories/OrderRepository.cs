using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Hr.Domain.BonusOrders;
using MTK.Modules.Hr.Domain.CompensationOrders;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;
using MTK.Modules.Hr.Domain.SalaryDeductions;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.VacationReturnOrders;
using MTK.Modules.Hr.Domain.Warnings;
using Microsoft.EntityFrameworkCore;

using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Repositories;

internal sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(HrDbContext context) : base(context) { }

    public override async Task<List<Order>> SearchAsync(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria,
        string? searchTerm,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = ApplyFiltersAndSort(filters, sortCriteria);
        var pagedQuery = ApplyPages(baseQuery, page, pageSize);

        // Load all orders with common properties
        var query = pagedQuery
            .Include(o => o.CreatedBy);

        var orders = await query.ToListAsync(cancellationToken);
        var orderIds = orders.Select(o => o.Id).ToList();

        // Batch load navigation properties for each order type
        await Context.Set<EmploymentOrder>()
            .Where(eo => orderIds.Contains(eo.Id))
            .Include(eo => eo.JobApplication)
            .LoadAsync(cancellationToken);

        await Context.Set<OrderForChangeOfPosition>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<UnexcusedAbsenceOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<WarningOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<CompensationOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<UnpaidLeaveOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<EducationLeaveOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<VacationOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<BonusOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<SalaryDeduction>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<EmploymentStatusChangeOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        await Context.Set<VacationReturnOrder>()
            .Where(o => orderIds.Contains(o.Id))
            .Include(o => o.Employee)
            .LoadAsync(cancellationToken);

        return orders;
    }
}
