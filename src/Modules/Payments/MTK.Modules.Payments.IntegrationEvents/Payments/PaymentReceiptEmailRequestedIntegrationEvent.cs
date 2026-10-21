using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Payments;

public sealed class PaymentReceiptEmailRequestedIntegrationEvent : IntegrationEvent
{
    public PaymentReceiptEmailRequestedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid paymentId,
        Guid ownerId,
        string email,
        string ownerName,
        string companyName,
        string receiptNumber,
        string fileName,
        byte[] pdfContent)
        : base(integrationEventId, occurredOnUtc)
    {
        PaymentId = paymentId;
        OwnerId = ownerId;
        Email = email;
        OwnerName = ownerName;
        CompanyName = companyName;
        ReceiptNumber = receiptNumber;
        FileName = fileName;
        PdfContent = pdfContent;
    }

    public Guid PaymentId { get; }
    public Guid OwnerId { get; }
    public string Email { get; }
    public string OwnerName { get; }
    public string CompanyName { get; }
    public string ReceiptNumber { get; }
    public string FileName { get; }
    public byte[] PdfContent { get; }
}
