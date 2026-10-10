namespace MTK.Modules.Hr.Application.Jobs.GetJobById;

public class GetJobByIdResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
