namespace MTK.Common.Domain.Queries;

public interface IBaseListResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
}
