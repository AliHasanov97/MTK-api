using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.VacationPays;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.SearchVacationPays;

internal sealed class SearchVacationPaysQueryHandler
    : IQueryHandler<SearchVacationPaysQuery, SearchVacationPaysResponse>
{
    private readonly IVacationPayRepository _vacationPayRepository;

    public SearchVacationPaysQueryHandler(IVacationPayRepository vacationPayRepository)
    {
        _vacationPayRepository = vacationPayRepository;
    }

    public async Task<Result<SearchVacationPaysResponse>> Handle(
        SearchVacationPaysQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page ?? 1);
        var pageSize = Math.Max(1, request.PageSize ?? 20);
        var skip = (page - 1) * pageSize;

        // Parse filters from frontend
        VacationPayStatus? statusFilter = null;
        DateOnly? fromDate = null;
        DateOnly? toDate = null;

        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                if (filter.ColumnName == "Status" && filter.Comparison == QueryComparisonType.In)
                {
                    // Frontend sends array of status numbers, take first one for simplicity
                    if (filter.Value is System.Text.Json.JsonElement jsonElement &&
                        jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        var firstValue = jsonElement.EnumerateArray().FirstOrDefault();
                        if (firstValue.ValueKind == System.Text.Json.JsonValueKind.Number)
                        {
                            statusFilter = (VacationPayStatus)firstValue.GetInt32();
                        }
                    }
                    else if (filter.Value is int statusValue)
                    {
                        statusFilter = (VacationPayStatus)statusValue;
                    }
                }
                else if (filter.ColumnName == "DueDate")
                {
                    if (filter.Comparison == QueryComparisonType.GreaterThanOrEqual &&
                        filter.Value is string dateStr && DateOnly.TryParse(dateStr, out var date))
                    {
                        fromDate = date;
                    }
                    else if (filter.Comparison == QueryComparisonType.LessThanOrEqual &&
                             filter.Value is string dateStr2 && DateOnly.TryParse(dateStr2, out var date2))
                    {
                        toDate = date2;
                    }
                }
            }
        }

        var vacationPays = await _vacationPayRepository.GetAllAsync(
            statusFilter,
            fromDate,
            toDate,
            request.SearchTerm,
            skip,
            pageSize,
            cancellationToken);

        var totalCount = await _vacationPayRepository.CountAsync(
            statusFilter,
            fromDate,
            toDate,
            request.SearchTerm,
            cancellationToken);

        var response = vacationPays.Select(vp => new VacationPayResponse(
            vp.Id,
            vp.VacationOrderId,
            vp.EmployeeId,
            vp.EmployeeFullName,
            vp.AverageMonthlyWage,
            vp.AverageDailyWage,
            vp.VacationDays,
            vp.TotalAmount,
            vp.VacationStartDate,
            vp.VacationEndDate,
            vp.Status,
            vp.DueDate,
            vp.PaidAt,
            vp.PaymentMethod,
            vp.CreatedAt,
            vp.UpdatedAt)).ToList();

        return new SearchVacationPaysResponse(response, totalCount, page, pageSize);
    }
}
