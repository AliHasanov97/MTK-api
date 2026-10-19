using MediatR;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Charges.Queries.ExportAnnualPaymentReportPdf;

internal sealed class ExportAnnualPaymentReportPdfQueryHandler
    : IQueryHandler<ExportAnnualPaymentReportPdfQuery, ExportFileResult>
{
    private const string PdfContentType = "application/pdf";

    private readonly ISender _sender;
    private readonly IPropertyOwnershipRepository _propertyOwnershipRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAnnualPaymentReportPdfExportService _exportService;

    public ExportAnnualPaymentReportPdfQueryHandler(
        ISender sender,
        IPropertyOwnershipRepository propertyOwnershipRepository,
        ICompanyRepository companyRepository,
        IAnnualPaymentReportPdfExportService exportService)
    {
        _sender = sender;
        _propertyOwnershipRepository = propertyOwnershipRepository;
        _companyRepository = companyRepository;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportAnnualPaymentReportPdfQuery request,
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

        var labels = ownerships.ToDictionary(
            o => (o.ApartmentId ?? o.GarageId)!.Value,
            o => o.GarageId.HasValue
                ? new PropertyExportLabel(o.Garage?.GarageNumber ?? "—", null, o.Owner?.FullName)
                : new PropertyExportLabel(o.Apartment?.ApartmentNumber ?? "—", o.Apartment?.Building?.Name, o.Owner?.FullName));

        var company = await _companyRepository.GetAsync(cancellationToken);
        var stream = _exportService.ExportToPdf(
            reportResult.Value,
            labels,
            company?.Name ?? "MTK");

        var typePart = request.PropertyType switch
        {
            PropertyType.Apartment => "Menzil_",
            PropertyType.Garage => "Qaraj_",
            _ => "",
        };

        var fileName = $"Illik_Odenis_Qrafiki_{typePart}{request.Year}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, PdfContentType);
    }
}
