using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.VendorPayments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IVendorPaymentRepository : IRepository<VendorPayment>
{
    Task<List<VendorPayment>> ListByChargeAsync(Guid vendorChargeId, CancellationToken cancellationToken = default);

    Task<List<VendorPayment>> ListByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);
}
