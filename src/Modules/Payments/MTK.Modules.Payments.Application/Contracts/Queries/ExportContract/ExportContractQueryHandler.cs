using MediatR;
using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Services.Export;
using MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;

namespace MTK.Modules.Payments.Application.Contracts.Queries.ExportContract;

internal sealed class ExportContractQueryHandler : IQueryHandler<ExportContractQuery, ExportFileResult>
{
    private readonly ISender _sender;
    private readonly IContractExportService _exportService;

    public ExportContractQueryHandler(ISender sender, IContractExportService exportService)
    {
        _sender = sender;
        _exportService = exportService;
    }

    public async Task<Result<ExportFileResult>> Handle(
        ExportContractQuery request,
        CancellationToken cancellationToken)
    {
        var contractResult = await _sender.Send(new GetContractByIdQuery(request.ContractId), cancellationToken);
        if (contractResult.IsFailure)
        {
            return Result.Failure<ExportFileResult>(contractResult.Error);
        }

        var stream = _exportService.ExportToPdf(contractResult.Value);
        var fileName = $"Muqavile_{contractResult.Value.Number}.pdf";

        return new ExportFileResult(stream.ToArray(), fileName, "application/pdf");
    }
}
