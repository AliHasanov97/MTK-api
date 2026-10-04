using Microsoft.EntityFrameworkCore;
using MTK.Common.Domain.Queries;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

/// <summary>
/// Tədarükçü borcları üzrə repository. Borclar tək <see cref="Charge"/> aqreqatında
/// olduğu üçün bütün sorğular <c>VendorId != null</c> ilə məhdudlaşdırılır.
/// </summary>
internal sealed class VendorChargeRepository : SearchableRepository<Charge>, IVendorChargeRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public VendorChargeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    // Axtarış/toplu sorğular avtomatik olaraq yalnız tədarükçü borclarını qaytarsın.
    protected override IQueryable<Charge> ApplyFiltersAndSort(
        List<QueryFilter>? filters,
        SortCriteria? sortCriteria)
    {
        return base.ApplyFiltersAndSort(filters, sortCriteria)
            .Where(c => c.VendorId != null);
    }

    public Task<bool> ExistsForPeriodAsync(
        Guid contractServiceId,
        string period,
        CancellationToken cancellationToken = default)
    {
        // Ləğv edilmiş borc yenidən hesablana bilsin deyə Cancelled nəzərə alınmır.
        return PaymentsContext.Charges
            .AnyAsync(c =>
                c.VendorId != null &&
                c.ContractServiceId == contractServiceId &&
                c.Period == period &&
                c.Status != ChargeStatus.Cancelled,
                cancellationToken);
    }

    public Task<List<Charge>> ListByVendorAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Charges
            .Where(c => c.VendorId == vendorId)
            .OrderByDescending(c => c.IssuedOn)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Charge>> ListByContractAsync(
        Guid contractId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Charges
            .Where(c => c.VendorId != null && c.ContractId == contractId)
            .OrderByDescending(c => c.IssuedOn)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Charge>> GetUnpaidChargesAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.Charges
            .Where(c =>
                c.VendorId == vendorId &&
                c.Status != ChargeStatus.Paid &&
                c.Status != ChargeStatus.Cancelled)
            .OrderBy(c => c.IssuedOn)
            .ThenBy(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }
}
