using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.FileAttachments;

namespace MTK.Modules.Payments.Domain.Repositories;

public interface IFileAttachmentRepository : IRepository<FileAttachment>
{
    Task<List<FileAttachment>> ListByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByPurchaseIdAsync(Guid purchaseId, CancellationToken cancellationToken = default);
    Task<List<FileAttachment>> ListByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
