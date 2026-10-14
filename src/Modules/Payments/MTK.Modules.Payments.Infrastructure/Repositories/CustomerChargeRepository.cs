using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

/// <summary>
/// Müştəri borcları üzrə repository. Borclar tək <see cref="Charge"/> aqreqatında
/// olduğu üçün bütün sorğular <c>CustomerId != null</c> ilə məhdudlaşdırılır.
/// </summary>
internal sealed class CustomerChargeRepository : SearchableRepository<Charge>, ICustomerChargeRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public CustomerChargeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<Charge> ApplyFiltersAndSort(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria)
    {
        return base.ApplyFiltersAndSort(filters, sortCriteria)
            .Where(c => c.CustomerId != null);
    }

    public async Task<HashSet<Guid>> GetBilledServiceIdsForPeriodAsync(
        string period,
        bool includeCancelled,
        CancellationToken cancellationToken = default)
    {
        var ids = await PaymentsContext.Charges
            .Where(c =>
                c.CustomerId != null &&
                c.ContractServiceId != null &&
                c.Period == period &&
                (includeCancelled || c.Status != ChargeStatus.Cancelled))
            .Select(c => c.ContractServiceId!.Value)
            .ToListAsync(cancellationToken);

        return [.. ids];
    }

    public Task<List<Charge>> ListByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Charges
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.IssuedOn)
            .ToListAsync(cancellationToken);
    }
}
