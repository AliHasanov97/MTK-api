namespace MTK.Modules.Hr.Application.Jobs.UpdateJob;

public class UpdateJobResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
