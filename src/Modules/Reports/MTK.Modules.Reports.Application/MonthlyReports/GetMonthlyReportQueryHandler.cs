using MediatR;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;
using MTK.Modules.Warehouse.Application.Reports.GetWarehouseMonthlyReport;

namespace MTK.Modules.Reports.Application.MonthlyReports;

/// <summary>
/// Hesabat modulu digər modulların bazalarına birbaşa qoşulmur — hər bölməni
/// öz modulunun read query-si ilə alır. Bu, modul sərhədlərini qoruyur və
/// gələcəkdə bölmələrin ayrıca servisə çıxarılmasını asanlaşdırır.
/// </summary>
internal sealed class GetMonthlyReportQueryHandler
    : IQueryHandler<GetMonthlyReportQuery, MonthlyReportResponse>
{
    private readonly ISender _sender;

    public GetMonthlyReportQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<Result<MonthlyReportResponse>> Handle(
        GetMonthlyReportQuery request,
        CancellationToken cancellationToken)
    {
        var financeResult = await _sender.Send(
            new GetMonthlyFinancialReportQuery(request.Year, request.Month),
            cancellationToken);

        if (financeResult.IsFailure)
        {
            return Result.Failure<MonthlyReportResponse>(financeResult.Error);
        }

        var warehouseResult = await _sender.Send(
            new GetWarehouseMonthlyReportQuery(request.Year, request.Month),
            cancellationToken);

        if (warehouseResult.IsFailure)
        {
            return Result.Failure<MonthlyReportResponse>(warehouseResult.Error);
        }

        return new MonthlyReportResponse(
            financeResult.Value,
            warehouseResult.Value,
            DateTimeOffset.UtcNow);
    }
}
