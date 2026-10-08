using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.FileAttachments;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.FileAttachments.Queries.ListFileAttachments;

internal sealed class ListFileAttachmentsQueryHandler : IQueryHandler<ListFileAttachmentsQuery, List<FileAttachmentResponse>>
{
    private readonly IFileAttachmentRepository _fileAttachmentRepository;

    public ListFileAttachmentsQueryHandler(IFileAttachmentRepository fileAttachmentRepository)
    {
        _fileAttachmentRepository = fileAttachmentRepository;
    }

    public async Task<Result<List<FileAttachmentResponse>>> Handle(
        ListFileAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        List<FileAttachment> attachments = request switch
        {
            { ContractId: { } contractId } => await _fileAttachmentRepository.ListByContractIdAsync(contractId, cancellationToken),
            { VendorId: { } vendorId } => await _fileAttachmentRepository.ListByVendorIdAsync(vendorId, cancellationToken),
            { PaymentId: { } paymentId } => await _fileAttachmentRepository.ListByPaymentIdAsync(paymentId, cancellationToken),
            { OwnerId: { } ownerId } => await _fileAttachmentRepository.ListByOwnerIdAsync(ownerId, cancellationToken),
            { PurchaseId: { } purchaseId } => await _fileAttachmentRepository.ListByPurchaseIdAsync(purchaseId, cancellationToken),
            { TransactionId: { } transactionId } => await _fileAttachmentRepository.ListByTransactionIdAsync(transactionId, cancellationToken),
            _ => [],
        };

        return attachments
            .Select(a => new FileAttachmentResponse(
                a.Id,
                a.FileName,
                a.ContentType,
                a.SizeBytes,
                a.CreatedAt,
                a.UploadedByUserId.HasValue && a.UploadedByUserId == request.RequestedByUserId))
            .ToList();
    }
}
