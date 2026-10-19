using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Reporting;

namespace MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;

internal sealed class GetMonthlyFinancialReportQueryHandler
    : IQueryHandler<GetMonthlyFinancialReportQuery, MonthlyFinancialReportResponse>
{
    private readonly IMonthlyFinancialReportReader _reader;

    public GetMonthlyFinancialReportQueryHandler(IMonthlyFinancialReportReader reader)
    {
        _reader = reader;
    }

    public async Task<Result<MonthlyFinancialReportResponse>> Handle(
        GetMonthlyFinancialReportQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Year is < 2000 or > 2200)
        {
            return Result.Failure<MonthlyFinancialReportResponse>(new Error(
                "MonthlyFinancialReport.InvalidYear",
                "İl 2000-2200 aralığında olmalıdır"));
        }

        if (request.Month is < 1 or > 12)
        {
            return Result.Failure<MonthlyFinancialReportResponse>(new Error(
                "MonthlyFinancialReport.InvalidMonth",
                "Ay 1-12 aralığında olmalıdır"));
        }

        var report = await _reader.ReadAsync(request.Year, request.Month, cancellationToken);
        return report;
    }
}
