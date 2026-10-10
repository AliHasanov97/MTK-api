using MTK.Modules.Hr.Domain.Employees;
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

    /// <summary>
    /// Əsas Order cədvəlində EmployeeId sütunu yoxdur (hər əmr növünün öz EmployeeId-si var).
    /// Ona görə Filters-dəki "EmployeeId" şərti, bütün növlərdən həmin işçiyə aid ID-lərin alt-sorğusuna çevrilir.
    /// </summary>
    protected override IQueryable<Order> ApplyFiltersAndSort(List<QueryFilter>? filters, SortCriteria? sortCriteria)
    {
        var employeeFilter = filters?.FirstOrDefault(f => f.ColumnName.Equals("EmployeeId", StringComparison.OrdinalIgnoreCase));
        if (employeeFilter is null)
            return base.ApplyFiltersAndSort(filters, sortCriteria);

        var employeeId = EmployeeFilterValue.ToGuid(employeeFilter.Value);
        var query = base.ApplyFiltersAndSort(filters!.Where(f => f != employeeFilter).ToList(), sortCriteria);

        // İşçi kartı işə qəbul əmrinə (Employee.EmploymentOrderId) bağlıdır
        var employmentOrderIds = Context.Set<Employee>()
            .Where(e => e.Id == employeeId && e.EmploymentOrderId != null)
            .Select(e => e.EmploymentOrderId!.Value);
        var ids = employmentOrderIds
            .Union(Context.Set<OrderForChangeOfPosition>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<UnexcusedAbsenceOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<WarningOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<CompensationOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<UnpaidLeaveOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<VacationOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<EducationLeaveOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<BonusOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<SalaryDeduction>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<EmploymentStatusChangeOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id))
            .Union(Context.Set<VacationReturnOrder>().Where(o => o.EmployeeId == employeeId).Select(o => o.Id));

        return query.Where(o => ids.Contains(o.Id));
    }

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
