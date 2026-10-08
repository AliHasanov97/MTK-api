using Microsoft.EntityFrameworkCore;
using MTK.Common.Infrastructure.Database;
using MTK.Modules.Payments.Domain.FileAttachments;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Repositories;

internal sealed class FileAttachmentRepository : Repository<FileAttachment>, IFileAttachmentRepository
{
    private PaymentsDbContext PaymentsContext => (PaymentsDbContext)Context;

    public FileAttachmentRepository(PaymentsDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<FileAttachment>> ListByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.ContractId == contractId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.VendorId == vendorId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.PaymentId == paymentId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.OwnerId == ownerId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByPurchaseIdAsync(Guid purchaseId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.PurchaseId == purchaseId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<FileAttachment>> ListByTransactionIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return PaymentsContext.FileAttachments
            .Where(f => f.TransactionId == transactionId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
