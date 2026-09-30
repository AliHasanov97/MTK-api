using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VendorCharges;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class VendorChargeRepository : SearchableRepository<VendorCharge>, IVendorChargeRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public VendorChargeRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<bool> ExistsForPeriodAsync(
        Guid contractServiceId,
        string period,
        CancellationToken cancellationToken = default)
    {
        // Ləğv edilmiş borc yenidən hesablana bilsin deyə Cancelled nəzərə alınmır.
        return PaymentsContext.VendorCharges
            .AnyAsync(c =>
                c.ContractServiceId == contractServiceId &&
                c.Period == period &&
                c.Status != VendorChargeStatus.Cancelled,
                cancellationToken);
    }

    public Task<bool> ExistsForReferenceAsync(
        Guid contractGoodsItemId,
        string reference,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VendorCharges
            .AnyAsync(c =>
                c.ContractGoodsItemId == contractGoodsItemId &&
                c.Reference == reference &&
                c.Status != VendorChargeStatus.Cancelled,
                cancellationToken);
    }

    public Task<List<VendorCharge>> ListByVendorAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VendorCharges
            .Where(c => c.VendorId == vendorId)
            .OrderByDescending(c => c.ChargeDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<VendorCharge>> ListByContractAsync(
        Guid contractId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VendorCharges
            .Where(c => c.ContractId == contractId)
            .OrderByDescending(c => c.ChargeDate)
            .ToListAsync(cancellationToken);
    }
}
