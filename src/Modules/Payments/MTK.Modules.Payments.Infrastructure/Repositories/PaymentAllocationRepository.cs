using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class PaymentAllocationRepository : Repository<PaymentAllocation>, IPaymentAllocationRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public PaymentAllocationRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<PaymentAllocation>> GetByChargeIdAsync(
        Guid chargeId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PaymentAllocations
            .Where(a => a.ChargeId == chargeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAllocation>> GetByPaymentIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PaymentAllocations
            .Where(a => a.PaymentId == paymentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PaymentAllocation>> GetByPaymentIdsAsync(
        List<Guid> paymentIds,
        CancellationToken cancellationToken = default)
    {
        return await PaymentsContext.PaymentAllocations
            .Where(a => paymentIds.Contains(a.PaymentId))
            .ToListAsync(cancellationToken);
    }
}
