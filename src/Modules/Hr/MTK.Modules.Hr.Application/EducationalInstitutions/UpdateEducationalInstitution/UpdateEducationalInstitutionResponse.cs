namespace MTK.Modules.Hr.Application.EducationalInstitutions.UpdateEducationalInstitution;

public class UpdateEducationalInstitutionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
