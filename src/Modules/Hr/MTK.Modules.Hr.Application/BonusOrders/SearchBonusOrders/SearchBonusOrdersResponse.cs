using MTK.Common.Presentation.Responses;
using MTK.Common.Domain.Queries;

namespace MTK.Modules.Hr.Application.BonusOrders.SearchBonusOrders;

public class SearchBonusOrdersResponse(
    List<SearchBonusOrdersResponseItem> data,
    int totalCount,
    int page,
    int pageSize)
    : PagedListResponse<SearchBonusOrdersResponseItem>(data, totalCount, page, pageSize);

public class SearchBonusOrdersResponseItem
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public ResponseObjectWithName Employee { get; set; } = null!;
    public ResponseObjectWithName OrderExecutionSupervisor { get; set; } = null!;
    public int BonusQuantity { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}
