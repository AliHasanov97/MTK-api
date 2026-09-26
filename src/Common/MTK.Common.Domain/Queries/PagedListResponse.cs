namespace MTK.Common.Domain.Queries;

public class PagedListResponse<T> : IBaseListResponse<T>
{
    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int TotalCount { get; set; }

    public PagedListResponse(List<T> data, int totalCount, int page = 1, int pageSize = 20)
    {
        Data = data;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
