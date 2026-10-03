using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Payments.Queries.ExportPaymentReceipt;

internal sealed class ExportPaymentReceiptQueryHandler : IQueryHandler<ExportPaymentReceiptQuery, ExportFileResult>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentAllocationRepository _paymentAllocationRepository;
    private readonly IChargeRepository _chargeRepository;
    private readonly IPaymentReceiptExportService _exportService;

    public ExportPaymentReceiptQueryHandler(
        IPaymentRepository paymentRepository,
        IPaymentAllocationRepository paymentAllocationRepository,
        IChargeRepository chargeRepository,
        IPaymentReceiptExportService exportService)
    {
        _paymentRepository = paymentRepository;
        _paymentAllocationRepository = paymentAllocationRepository;
        _chargeRepository = chargeRepository;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportPaymentReceiptQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdDefaultAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<ExportFileResult>(new Error(
                "Payment.NotFound",
                $"Ödəniş tapılmadı: {request.PaymentId}"));
        }

        var allocations = await _paymentAllocationRepository.GetByPaymentIdAsync(payment.Id, cancellationToken);
        var chargeIds = allocations.Select(a => a.ChargeId).Distinct().ToList();
        var chargesById = (await _chargeRepository.ListFromIdsAsync(chargeIds, cancellationToken))
            .ToDictionary(c => c.Id);

        var lines = allocations
            .Select(a => new PaymentReceiptLine(
                chargesById.TryGetValue(a.ChargeId, out var charge) ? charge.Description ?? "Haqq" : "Haqq",
                a.Amount))
            .ToList();

        var data = new PaymentReceiptData(
            payment.Id,
            payment.Amount,
            payment.PaymentMethod,
            payment.PaymentDate,
            payment.Notes,
            payment.PartyType,
            request.PayerName,
            request.PropertyLabel,
            lines);

        var stream = _exportService.ExportToPdf(data);
        var fileName = $"Qebz_{payment.PaymentDate:yyyy-MM-dd}_{payment.Id.ToString()[..8]}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
