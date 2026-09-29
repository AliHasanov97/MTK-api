using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Payments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IPaymentAllocationRepository : IRepository<PaymentAllocation>
{
    Task<List<PaymentAllocation>> GetByChargeIdAsync(Guid chargeId, CancellationToken cancellationToken = default);
    Task<List<PaymentAllocation>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<List<PaymentAllocation>> GetByPaymentIdsAsync(List<Guid> paymentIds, CancellationToken cancellationToken = default);
}
