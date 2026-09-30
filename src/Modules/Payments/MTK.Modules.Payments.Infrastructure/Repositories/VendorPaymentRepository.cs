using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VendorPayments;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class VendorPaymentRepository : SearchableRepository<VendorPayment>, IVendorPaymentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public VendorPaymentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<VendorPayment>> ListByChargeAsync(
        Guid vendorChargeId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VendorPayments
            .Where(p => p.VendorChargeId == vendorChargeId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<VendorPayment>> ListByVendorAsync(
        Guid vendorId,
        CancellationToken cancellationToken = default)
    {
        return PaymentsContext.VendorPayments
            .Where(p => p.VendorId == vendorId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }
}
