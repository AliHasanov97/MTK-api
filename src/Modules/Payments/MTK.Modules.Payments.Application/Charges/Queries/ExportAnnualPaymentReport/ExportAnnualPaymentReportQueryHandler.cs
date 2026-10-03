using MediatR;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.ExportAnnualPaymentReport;

internal sealed class ExportAnnualPaymentReportQueryHandler : IQueryHandler<ExportAnnualPaymentReportQuery, ExportFileResult>
{
    private readonly ISender _sender;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly IAnnualPaymentReportExcelExportService _exportService;

    public ExportAnnualPaymentReportQueryHandler(
        ISender sender,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        IAnnualPaymentReportExcelExportService exportService)
    {
        _sender = sender;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportAnnualPaymentReportQuery request,
        CancellationToken cancellationToken)
    {
        var reportResult = await _sender.Send(
            new GetAnnualPaymentReportQuery(request.Year, request.PropertyType),
            cancellationToken);

        if (reportResult.IsFailure)
        {
            return Result.Failure<ExportFileResult>(reportResult.Error);
        }

        var ownerships = request.PropertyType is { } propertyType
            ? await _propertyOwnershipRepository.GetByPropertyTypeAsync(propertyType, cancellationToken)
            : await _propertyOwnershipRepository.GetAllWithOwnersAsync(cancellationToken);

        var propertyLabels = ownerships.ToDictionary(
            o => o.PropertyId,
            o => $"{o.PropertyNumber ?? "—"} ({(o.PropertyType == PropertyType.Garage ? "Qaraj" : "Mənzil")})");

        var stream = _exportService.ExportToExcel(reportResult.Value, propertyLabels);
        var fileName = $"Illik_Hesabat_{request.Year}.xlsx";

        return new ExportFileResult(
            stream.ToArray(),
            fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}
