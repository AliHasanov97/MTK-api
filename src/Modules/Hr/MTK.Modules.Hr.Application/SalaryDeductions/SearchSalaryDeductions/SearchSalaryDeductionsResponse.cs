using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.SalaryDeductions.SearchSalaryDeductions;

public class SearchSalaryDeductionsResponse(
    List<SearchSalaryDeductionsResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchSalaryDeductionsResponseItem>(data, totalCount, page, pageSize);

public class SearchSalaryDeductionsResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public string District { get; set; } = string.Empty;
    public int JudgementNo { get; set; }
    public DateTimeOffset JudgementDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public int PercentageSalary { get; set; }
    public decimal StateFee { get; set; }
    public string Creditor { get; set; } = string.Empty;
    public decimal Debt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}
