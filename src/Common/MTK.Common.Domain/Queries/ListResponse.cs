namespace MTK.Common.Domain.Queries;

public abstract class ListResponse<T>(List<T> data, int totalCount) : IBaseListResponse<T>
{
    public List<T> Data { get; set; } = data;
    public int TotalCount { get; set; } = totalCount;
}
