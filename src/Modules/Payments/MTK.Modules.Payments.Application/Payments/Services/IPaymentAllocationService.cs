using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.Payments.Services;

public interface IPaymentAllocationService
{
    Task<Result> AllocatePaymentAsync(Guid paymentId, Guid ownerId, decimal amount, CancellationToken cancellationToken);
}
